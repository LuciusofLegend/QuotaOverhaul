using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;

namespace QuotaOverhaul
{
    public class SaveLootPatch
    {
        private static readonly Type patchType;

        static SaveLootPatch()
        {
            patchType = typeof(SaveLootPatch);
            Plugin.harmony.Patch(AccessTools.Method(typeof(RoundManager), "DespawnPropsAtEndOfRound", (Type[])null, (Type[])null), transpiler: new HarmonyMethod(patchType, "RM_DespawnPropsAtEndOfRound_Transpiler", (Type[])null));
        }

        public static IEnumerable<CodeInstruction> RM_DespawnPropsAtEndOfRound_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            int startIndex = -1;
            int endIndex = -1;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_0)
                {
                    startIndex = i;
                    for (int j = startIndex + 1; j < codes.Count; j++)
                    {
                        if (codes[j].Is(OpCodes.Ldstr, "TemporaryEffect"))
                        {
                            endIndex = j;
                            break;
                        }
                    }
                    if (endIndex > -1)
                    {
                        break;
                    }
                }
            }
            if (startIndex > -1 && endIndex > -1)
            {
                Label labelSkip = il.DefineLabel();
                codes[endIndex].labels.Add(labelSkip);
                List<CodeInstruction> instructionsToInsert = new List<CodeInstruction>();
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_1));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SaveLootPatch), "CustomDespawnProps")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brtrue_S, labelSkip));
                codes.InsertRange(startIndex + 1, instructionsToInsert);
            }
            return codes.AsEnumerable();
        }

        public static bool CustomDespawnProps(RoundManager rManager, GrabbableObject[] gObjects, bool despawnAllItems = false)
        {
            if (despawnAllItems)
            {
                return false;
            }
            System.Random RNG = new System.Random(StartOfRound.Instance.randomMapSeed + 369);
            List<GrabbableObject> gObjectsAll = gObjects.ToList();
            List<GrabbableObject> gObjectsInside = new List<GrabbableObject>();
            foreach (GrabbableObject gObject in gObjects)
            {
                if (!(gObject.isInShipRoom ||  gObject.isHeld) || gObject.deactivated)
                {
                    Plugin.Log.LogInfo($"{gObject.name} Lost Outside");
                    DespawnItem(gObject);
                }
                else
                {
                    gObjectsInside.Add(gObject);
                }
            }

            var result = gObjectsInside.ToLookup((GrabbableObject go) => go.itemProperties.isScrap);
            List<GrabbableObject> gObjectsScrap = result[true].ToList();
            List<GrabbableObject> gObjectsEquipment = result[false].ToList();

            if (StartOfRound.Instance.allPlayersDead)
            {
                if (RNG.NextDouble() >= (1f - (Config.saveAllChance?.Value ?? 0.25f)))
                {
                    Plugin.Log.LogInfo("All Saved");
                }
                else
                {
                    gObjectsScrap.RemoveAll((GrabbableObject go) => !go.IsSpawned);
                    if (Config.valueSaveEnabled?.Value ?? false)
                    {
                        gObjectsScrap = gObjectsScrap.OrderByDescending((GrabbableObject go) => go.scrapValue).ToList();
                        int totalScrap = gObjectsScrap.Sum((GrabbableObject go) => go.scrapValue);
                        float saveScrap = totalScrap * (Config.valueSavePercent?.Value ?? 0.25f);
                        foreach (GrabbableObject gObject in gObjectsScrap)
                        {
                            totalScrap -= gObject.scrapValue;
                            Plugin.Log.LogInfo($"{gObject.name} Lost Value {gObject.scrapValue}");
                            DespawnItem(gObject);
                            if (totalScrap < saveScrap)
                            {
                                Plugin.Log.LogInfo($"{totalScrap} Scrap Value Saved");
                                break;
                            }
                        }
                    }
                    else
                    {
                        int lostSCount = 0;
                        foreach (GrabbableObject gObject in gObjectsScrap)
                        {
                            if (RNG.NextDouble() >= (1f - (Config.saveEachChance?.Value ?? 0.5f)))
                            {
                                Plugin.Log.LogInfo($"{gObject.name} Saved");
                            }
                            else
                            {
                                Plugin.Log.LogInfo($"{gObject.name} Lost");
                                DespawnItem(gObject);
                                lostSCount++;
                                if (lostSCount >= (Config.scrapLossMax?.Value ?? int.MaxValue))
                                {
                                    Plugin.Log.LogInfo($"Lost total {lostSCount}");
                                    break;
                                }
                            }
                        }
                    }
                    if (Config.equipmentLossEnabled?.Value ?? false)
                    {
                        gObjectsEquipment.RemoveAll((GrabbableObject go) => !go.IsSpawned);
                        int lostECount = 0;
                        foreach (GrabbableObject gObject in gObjectsEquipment)
                        {
                            if (RNG.NextDouble() >= (1f - (Config.equipmentLossChance?.Value ?? 0.1f)))
                            {
                                Plugin.Log.LogInfo($"{gObject.name} Equipment Lost");
                                DespawnItem(gObject);
                                lostECount++;
                                if (lostECount >= (Config.equipmentLossMax?.Value ?? int.MaxValue))
                                {
                                    Plugin.Log.LogInfo($"Equipment Lost total {lostECount}");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return true;

            void DespawnItem(GrabbableObject gObject)
            {
                if (gObject.isHeld && gObject.playerHeldBy != null)
                {
                    gObject.playerHeldBy.DropAllHeldItems();
                }
                gObject.gameObject.GetComponent<NetworkObject>().Despawn(true);
                if (rManager.spawnedSyncedObjects.Contains(gObject.gameObject))
                {
                    rManager.spawnedSyncedObjects.Remove(gObject.gameObject);
                }
            }
        }
    }
}