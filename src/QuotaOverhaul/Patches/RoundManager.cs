using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace QuotaOverhaul.Patches
{
    [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
    public class DespawnPropsPatch
    {
        public static bool Prefix()
        {
            return false;
        }
        
        public static void Postfix(bool despawnAllItems = false)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            VehicleController[] vehicles = UnityEngine.Object.FindObjectsOfType<VehicleController>();
            foreach (VehicleController vehicle in vehicles)
            {
                DespawnVehicle(vehicle);
            }

            BeltBagItem[] beltBags = UnityEngine.Object.FindObjectsOfType<BeltBagItem>();
            foreach (BeltBagItem beltBag in beltBags)
            {
                if (beltBag.insideAnotherBeltBag && (beltBag.insideAnotherBeltBag.isInShipRoom || beltBag.insideAnotherBeltBag.isHeld))
                {
                    beltBag.isInElevator = true;
                    beltBag.isInShipRoom = true;
                }
                if (beltBag.isInShipRoom || beltBag.isHeld)
                {
                    foreach (GrabbableObject objectInBag in beltBag.objectsInBag)
                    {
                        objectInBag.isInElevator = true;
                        objectInBag.isInShipRoom = true;
                    }
                }
            }

            System.Random rng = new System.Random(StartOfRound.Instance.randomMapSeed + 369);
            List<GrabbableObject> items = UnityEngine.Object.FindObjectsOfType<GrabbableObject>().ToList();
            List<GrabbableObject> itemsInside = [];

            if (despawnAllItems)
            {
                Plugin.Log.LogInfo("Despawning all items");
                foreach (GrabbableObject item in items)
                {
                    DespawnItem(item);
                }
                return;
            }

            foreach (GrabbableObject item in items)
            {
                if (item == null) continue;

                if (!(item.isInShipRoom || item.isHeld) || item.deactivated)
                {
                    Plugin.Log.LogInfo($"{item.itemProperties.itemName ?? item.name} Lost Outside");
                    DespawnItem(item);
                }
                else
                {
                    itemsInside.Add(item);
                }
            }

            if (!StartOfRound.Instance.allPlayersDead) return;

            ILookup<bool, GrabbableObject> itemIsScrapLookup = itemsInside.ToLookup(item => item.itemProperties.isScrap);
            List<GrabbableObject> itemsScrap = itemIsScrapLookup[true].ToList();
            List<GrabbableObject> itemsEquipment = itemIsScrapLookup[false].ToList();
            List<string> lostItems = [];

            bool itemsAreSafe = rng.NextDouble() < Config.ItemsSafeChance.Value / 100;

            if (!Config.ScrapLossEnabled.Value)
            {
                Plugin.Log.LogInfo("Scrap loss is disabled");
            }
            else if (!itemsAreSafe)
            {
                itemsScrap.RemoveAll(item => !item.IsSpawned);
                int totalScrapValue = itemsScrap.Sum(scrap => scrap.scrapValue);
                int scrapLost = 0;
                int scrapValueLost = 0;

                if (Config.ValueLossEnabled.Value)
                {
                    itemsScrap = itemsScrap.OrderByDescending(scrap => scrap.scrapValue).ToList();
                    int valueToLose = (int)(totalScrapValue * Config.ValueLossPercent.Value / 100);
                    foreach (GrabbableObject scrap in itemsScrap)
                    {
                        if (scrapValueLost >= valueToLose || scrapLost >= Config.MaxLostScrapItems.Value) break;
                        scrapValueLost += scrap.scrapValue;
                        scrapLost++;
                        lostItems.Add(scrap.itemProperties?.itemName ?? scrap.name);
                        DespawnItem(scrap);
                        Plugin.Log.LogInfo($"Lost {scrap.name} worth {scrap.scrapValue}");
                    }
                    itemsScrap.RemoveAll(item => !item.IsSpawned);
                    Plugin.Log.LogInfo($"Value Loss: {scrapValueLost}$ of scrap lost");
                }

                foreach (GrabbableObject scrap in itemsScrap)
                {
                    if (rng.NextDouble() < Config.LoseEachScrapChance.Value / 100)
                    {
                        if (scrapLost >= Config.MaxLostScrapItems.Value) break;
                        scrapValueLost += scrap.scrapValue;
                        scrapLost++;
                        lostItems.Add(scrap.itemProperties?.itemName ?? scrap.name);
                        DespawnItem(scrap);
                        Plugin.Log.LogInfo($"Lost {scrap.name} worth {scrap.scrapValue}");
                    }
                }

                Plugin.Log.LogInfo($"Lost {scrapLost} scrap items worth {scrapValueLost}");
            }

            if (!Config.EquipmentLossEnabled.Value)
            {
                Plugin.Log.LogInfo("Equipment loss is disabled");
            }
            else if (!itemsAreSafe)
            {
                itemsEquipment.RemoveAll(item => !item.IsSpawned);
                int equipmentLost = 0;
                foreach (GrabbableObject equipment in itemsEquipment)
                {
                    if (rng.NextDouble() < Config.LoseEachEquipmentChance.Value / 100)
                    {
                        equipmentLost++;
                        if (equipmentLost > Config.MaxLostEquipmentItems.Value) break;
                        lostItems.Add(equipment.itemProperties?.itemName ?? equipment.name);
                        DespawnItem(equipment);
                        Plugin.Log.LogInfo($"Lost {equipment.name}");
                    }
                }
                Plugin.Log.LogInfo($"Lost {equipmentLost} equipment items");
            }

            if (lostItems.Any())
            {
                string msg = $"Lost items ({lostItems.Count()}/{itemsInside.Count()}): ";
                msg += string.Join("; ", lostItems.GroupBy(s => s).Select(s => new { name = s.Key, count = s.Count() }).Select(item => item.count > 1 ? $"{item.name} x{item.count}" : item.name));
                HUDManager.Instance.StartCoroutine(DisplayAlert(bodyAlertText: "", messageText: msg));
            }
        }

        private static void DespawnVehicle(VehicleController vehicle)
        {
            try
            {
                if (!vehicle.magnetedToShip)
                {
                    if (vehicle.NetworkObject != null)
                    {
                        vehicle.NetworkObject.Despawn(false);
                        Plugin.Log.LogInfo("Despawned vehicle");
                    }
                }
                else
                {
                    vehicle.CollectItemsInTruck();
                }
            }
            catch (Exception arg)
            {
                Plugin.Log.LogError($"Error despawning vehicle: {arg}");
            }
        }

        private static void DespawnItem(GrabbableObject item)
        {
            if (item.isHeld && item.playerHeldBy != null)
            {
                item.playerHeldBy.DropAllHeldItemsAndSync();
            }
            NetworkObject networkComponent = item.gameObject.GetComponent<NetworkObject>();
            if (networkComponent != null && networkComponent.IsSpawned)
            {
                Plugin.Log.LogInfo($"Despawning {item.itemProperties.itemName ?? item.name}");
                networkComponent.Despawn();
            }
            else
            {
                Plugin.Log.LogDebug($"Error/warning: {item.itemProperties.itemName ?? item.itemProperties.itemName ?? item.name} was not spawned or did not have a NetworkObject component!  Skipped despawning and destroyed it instead.");
                UnityEngine.Object.Destroy(item.gameObject);
            }
            if (RoundManager.Instance.spawnedSyncedObjects.Contains(item.gameObject))
            {
                RoundManager.Instance.spawnedSyncedObjects.Remove(item.gameObject);
            }
        }

        private static IEnumerator DisplayAlert(string headerAlertText = "Quota Overhaul", string bodyAlertText = "", string messageText = "")
        {
            int index = 0;
            while (index < 20)
            {
                if (StartOfRound.Instance.inShipPhase)
                {
                    break;
                }
                index++;
                yield return new WaitForSeconds(5f);
            }
            yield return new WaitForSeconds(2f);
            if (!(string.IsNullOrEmpty(headerAlertText) && string.IsNullOrEmpty(bodyAlertText)))
            {
                HUDManager.Instance.DisplayTip(headerAlertText, bodyAlertText);
            }
            if (!string.IsNullOrEmpty(messageText))
            {
                HUDManager.Instance.AddTextToChatOnServer(messageText);
            }
        }
    }
}