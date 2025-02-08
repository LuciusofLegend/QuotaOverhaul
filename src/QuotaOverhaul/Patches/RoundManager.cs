using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
    public class SaveLootPatch
    {
        public static bool Prefix()
        {
            return false;
        }

        public static void Postfix(bool despawnAllItems = false)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            GrabbableObject[] items = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

            try
            {
                VehicleController[] vehicles = UnityEngine.Object.FindObjectsByType<VehicleController>(UnityEngine.FindObjectsSortMode.None);
                foreach (VehicleController vehicle in vehicles)
                {
                    if (!vehicle.magnetedToShip)
                    {
                        if (vehicle.NetworkObject != null)
                        {
                            Plugin.Log.LogInfo("Despawn vehicle");
                            vehicle.NetworkObject.Despawn(destroy: false);
                        }
                    }
                    else
                    {
                        vehicle.CollectItemsInTruck();
                    }
                }
            }
            catch (Exception arg)
            {
                Plugin.Log.LogError($"Error despawning vehicle: {arg}");
            }

            if (despawnAllItems)
            {
                foreach (GrabbableObject item in items)
                {
                    DespawnItem(item);
                }
                Plugin.Log.LogInfo("Despawned all props");
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
                int totalScrapValue = itemsScrap.Sum((GrabbableObject item) => item.scrapValue);
                float valueToSave = totalScrapValue * (Config.valueSavePercent?.Value ?? 25f) / 100;
                foreach (GrabbableObject item in itemsScrap)
                {
                    totalScrapValue -= item.scrapValue;
                    if (totalScrapValue < valueToSave)
                    {
                        Plugin.Log.LogInfo($"{totalScrapValue} Scrap Value Saved");
                        break;
                    }
                    Plugin.Log.LogInfo($"{item.name} Lost. Value: {item.scrapValue}");
                    DespawnItem(item);
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
                            Plugin.Log.LogInfo($"Lost {lostSCount} Scrap Items");
                            break;
                        }
                    }
                }
            }
            if (Config.equipmentLossEnabled?.Value ?? false)
            {
                itemsEquipment.RemoveAll((GrabbableObject go) => !go.IsSpawned);
                int lostEquipmentCount = 0;
                foreach (GrabbableObject item in itemsEquipment)
                {
                    if (lostEquipmentCount >= (Config.equipmentLossMax?.Value ?? int.MaxValue))
                    {
                        break;
                    }
                    if (RNG.NextDouble() >= (1f - (Config.equipmentLossChance?.Value ?? 10f) / 100))
                    {
                        Plugin.Log.LogInfo($"{item.name} Lost");
                        DespawnItem(item);
                        lostEquipmentCount++;
                    }
                }
                Plugin.Log.LogInfo($"Equipment Lost total {lostEquipmentCount}");
            }
        }

        public static void DespawnItem(GrabbableObject item)
        {
            if (item.isHeld && item.playerHeldBy != null)
            {
                item.playerHeldBy.DropAllHeldItemsAndSync();
            }
            NetworkObject networkComponent = item.gameObject.GetComponent<NetworkObject>();
            if (networkComponent != null && networkComponent.IsSpawned)
            {
                Plugin.Log.LogInfo($"Despawning {item.name}");
                networkComponent.Despawn();
            }
            else
            {
                Plugin.Log.LogDebug($"Error/warning: {item.name} prop was not spawned or did not have a NetworkObject component!  Skipped despawning and destroyed it instead.");
                UnityEngine.Object.Destroy(item.gameObject);
            }
            if (RoundManager.Instance.spawnedSyncedObjects.Contains(item.gameObject))
            {
                RoundManager.Instance.spawnedSyncedObjects.Remove(item.gameObject);
            }
        }
    }
}