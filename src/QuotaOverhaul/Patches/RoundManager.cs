/*using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
    public class SaveLootPatch
    {
        [HarmonyPrefix]
        public static void SkipOriginalDespawnProps()
        {
            return;
        }

        [HarmonyPostfix]
        public static void CustomDespawnProps(bool despawnAllItems = false)
        {
            GrabbableObject[] items = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

            if (despawnAllItems)
            {
                foreach (GrabbableObject item in items)
                {
                    DespawnItem(item);
                    Plugin.Log.LogInfo("Despawned all props");
                }
            }

            Random RNG = new Random(StartOfRound.Instance.randomMapSeed + 369);
            List<GrabbableObject> itemsInside = new List<GrabbableObject>();
            foreach (GrabbableObject item in items)
            {
                if (!(item.isInShipRoom || item.isHeld) || item.deactivated)
                {
                    Plugin.Log.LogInfo($"{item.name} Lost Outside");
                    DespawnItem(item);
                }
                else
                {
                    itemsInside.Add(item);
                }
            }

            if (!StartOfRound.Instance.allPlayersDead)
            {
                return;
            }
            if (RNG.NextDouble() >= (1f - (Config.saveAllChance?.Value ?? 25f) / 100))
            {
                Plugin.Log.LogInfo("All Saved");
                return;
            }

            var result = itemsInside.ToLookup((GrabbableObject go) => go.itemProperties.isScrap);
            List<GrabbableObject> itemsScrap = result[true].ToList();
            List<GrabbableObject> itemsEquipment = result[false].ToList();

            itemsScrap.RemoveAll((GrabbableObject item) => !item.IsSpawned);
            if (Config.valueSaveEnabled?.Value ?? false)
            {
                itemsScrap = itemsScrap.OrderByDescending((GrabbableObject item) => item.scrapValue).ToList();
                int totalScrap = itemsScrap.Sum((GrabbableObject item) => item.scrapValue);
                float saveScrap = totalScrap * (Config.valueSavePercent?.Value ?? 25f) / 100;
                foreach (GrabbableObject item in itemsScrap)
                {
                    totalScrap -= item.scrapValue;
                    Plugin.Log.LogInfo($"{item.name} Lost Value {item.scrapValue}");
                    DespawnItem(item);
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
                foreach (GrabbableObject item in itemsScrap)
                {
                    if (RNG.NextDouble() >= (1f - (Config.saveEachChance?.Value ?? 50f) / 100))
                    {
                        Plugin.Log.LogInfo($"{item.name} Saved");
                    }
                    else
                    {
                        Plugin.Log.LogInfo($"{item.name} Lost");
                        DespawnItem(item);
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
                itemsEquipment.RemoveAll((GrabbableObject go) => !go.IsSpawned);
                int lostECount = 0;
                foreach (GrabbableObject item in itemsEquipment)
                {
                    if (lostECount >= (Config.equipmentLossMax?.Value ?? int.MaxValue))
                    { 
                        break;
                    }
                    if (RNG.NextDouble() >= (1f - (Config.equipmentLossChance?.Value ?? 10f) / 100))
                    {
                        Plugin.Log.LogInfo($"{item.name} Equipment Lost");
                        DespawnItem(item);
                        lostECount++;
                    }
                }
                Plugin.Log.LogInfo($"Equipment Lost total {lostECount}");
            }
        }

        public static void DespawnItem(GrabbableObject item)
        {
            if (item.isHeld && item.playerHeldBy != null)
            {
                item.playerHeldBy.DropAllHeldItems();
            }
            item.gameObject.GetComponent<NetworkObject>().Despawn(true);
            if (RoundManager.Instance.spawnedSyncedObjects.Contains(item.gameObject))
            {
                RoundManager.Instance.spawnedSyncedObjects.Remove(item.gameObject);
            }
        }
    }
}*/