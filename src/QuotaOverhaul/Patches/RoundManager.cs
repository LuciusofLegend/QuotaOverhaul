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
            return !Plugin.Config.DespawnPropsPatch.Value;
        }

        public static void Postfix(bool despawnAllItems = false)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            if (!Plugin.Config.DespawnPropsPatch.Value)
            {
                Plugin.Log.LogInfo("Despawn Items patch is disabled.   The Vanilla scrap loss method will be used.");
                return;
            }

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

            System.Random rng = new(StartOfRound.Instance.randomMapSeed + 369);
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
            if (!Plugin.Config.ScrapLossOnGordion && StartOfRound.Instance.currentLevel.PlanetName == "71 Gordion")
            {
                Plugin.Log.LogInfo("Scrap Loss On Gordion is false.  Your items are safe at the company");
                return;
            }

            List<GrabbableObject> lostItems = DeathConsequences.DetermineLostItems(itemsInside);
            foreach (GrabbableObject item in lostItems) DespawnItem(item);
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
                item.playerHeldBy.DropAllHeldItemsAndSyncNonexact();
            }
            NetworkObject networkComponent = item.gameObject.GetComponent<NetworkObject>();
            if (networkComponent != null && networkComponent.IsSpawned)
            {
                Plugin.Log.LogInfo($"Despawning {item.itemProperties.itemName ?? item.name}");
                networkComponent.Despawn();
            }
            else
            {
                Plugin.Log.LogInfo($"Error/warning: {item.itemProperties.itemName ?? item.itemProperties.itemName ?? item.name} was not spawned or did not have a NetworkObject component!  Skipped despawning and destroyed it instead.");
                UnityEngine.Object.Destroy(item.gameObject);
            }
            if (RoundManager.Instance.spawnedSyncedObjects.Contains(item.gameObject))
            {
                RoundManager.Instance.spawnedSyncedObjects.Remove(item.gameObject);
            }
        }
    }
}
