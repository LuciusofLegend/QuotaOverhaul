using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace QuotaOverhaul
{
    public class DeathConsequences
    {

        public static void DoDeathConsequences(int deadBodies, int recoveredBodies)
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

            int oldCredits = terminal.groupCredits;
            double creditPenalty = CalculateCreditPenalty(deadBodies, recoveredBodies);

            if (GameNetworkManager.Instance.isHostingGame)
            {
                terminal.groupCredits -= (int)(oldCredits * creditPenalty);
                if (terminal.groupCredits < 0) terminal.groupCredits = 0;
            }

            int oldQuota = TimeOfDay.Instance.profitQuota;
            double quotaPenalty = 0d;

            quotaPenalty = CalculateQuotaPenalty(deadBodies, recoveredBodies);

            int creditPenaltyFromCombinedSystem = 0;
            if (Plugin.Config.ChargeCreditsInsteadOfQuota.Value)
            {
                Plugin.Log.LogDebug("Applying combined penalty system...");
                int creditsOwed = (int)(TimeOfDay.Instance.profitQuota * Plugin.Config.CreditsPerQuota.Value * quotaPenalty);
                Plugin.Log.LogDebug($"You owe {creditsOwed} credits");
                if (terminal.groupCredits >= creditsOwed) {
                    Plugin.Log.LogDebug($"You have {terminal.groupCredits} credits, which is enough to cover the penalty");
                    creditPenaltyFromCombinedSystem = creditsOwed;
                }
                else {
                    Plugin.Log.LogDebug($"You have {terminal.groupCredits} credits, which is NOT enough to cover the penalty")
                    creditPenaltyFromCombinedSystem = terminal.groupCredits;
                    double remainingPenalty = 1 / creditsOwed * terminal.groupCredits * quotaPenalty / Plugin.Config.CreditsPerQuota.Value;
                    quotaPenalty = remainingPenalty;
                    Plugin.Log.LogDebug($"You've lost all your credits and the remaining quota penalty is {quotaPenalty}");
                }
                terminal.groupCredits -= creditPenaltyFromCombinedSystem;
                if (terminal.groupCredits < 0) terminal.groupCredits = 0;
            }
            QuotaOverhaul.QuotaPenaltyMultiplier.Increase(quotaPenalty);

            string penaltyAdditionText = $"CASUALTIES: {deadBodies}\nBODIES RECOVERED: {recoveredBodies} \n \nCREDITS: -{(int)(creditPenalty * 100)}% -{creditPenaltyFromCombinedSystem} \n{oldCredits} -> {terminal.groupCredits} \n \nQUOTA: +{(int)(quotaPenalty * 100)}% \n{oldQuota} -> {TimeOfDay.Instance.profitQuota}";
            HUDManager.Instance.statsUIElements.penaltyAddition.text = penaltyAdditionText;
            HUDManager.Instance.statsUIElements.penaltyTotal.text = "";
        }

        public static double CalculateCreditPenalty(int deadBodies, int recoveredBodies)
        {
            if (!Plugin.Config.CreditPenaltiesEnabled.Value ||
                !(Plugin.Config.CreditPenaltiesOnGordion.Value || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion")) return 0;

            if (Plugin.Config.CreditPenaltiesDynamic.Value) return CalculateDynamicCreditPenalty(deadBodies, recoveredBodies);
            else return CalculateStaticCreditPenalty(deadBodies, recoveredBodies);
        }

        private static double CalculateStaticCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Plugin.Config.CreditPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Plugin.Config.CreditPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Credit Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.GetRecordPlayersThisMoon() * Plugin.Config.CreditPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.CreditPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Plugin.Config.CreditPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Credit Penalty of {penalty}");
            return penalty;
        }

        public static double CalculateQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            if (!Plugin.Config.QuotaPenaltiesEnabled.Value ||
                !(Plugin.Config.QuotaPenaltiesOnGordion.Value || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion")) return 0;

            if (Plugin.Config.QuotaPenaltiesDynamic.Value) return CalculateDynamicQuotaPenalty(deadBodies, recoveredBodies);
            else return CalculateStaticQuotaPenalty(deadBodies, recoveredBodies);
        }

        private static double CalculateStaticQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Plugin.Config.QuotaPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;
            Plugin.Log.LogInfo($"Calculated Quota Penalty of {penalty}");

            if (penalty < 0 || penalty < Plugin.Config.QuotaPenaltyPercentThreshold.Value / 100d)
                Plugin.Log.LogInfo($"Penalty fell below threshold of {Plugin.Config.QuotaPenaltyPercentThreshold.Value / 100d}");
            {
                penalty = 0;
            }

            return penalty;
        }

        private static double CalculateDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.GetRecordPlayersThisMoon() * Plugin.Config.QuotaPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;
            Plugin.Log.LogDebug($"Calculated Dynamic Quota Penalty of {penalty}");

            if (penalty < 0 || penalty < Plugin.Config.QuotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
                Plugin.Log.LogDebug($"Penalty fell below threshold of {Plugin.Config.QuotaPenaltyPercentThreshold.Value / 100d}.  No penalty will be applied.");
            }

            return penalty;
        }

        public static List<GrabbableObject> DetermineLostItems(List<GrabbableObject> items)
        {
            ILookup<bool, GrabbableObject> itemIsScrapLookup = items.ToLookup(item => item.itemProperties.isScrap);
            List<GrabbableObject> itemsScrap = [.. itemIsScrapLookup[true]];
            List<GrabbableObject> itemsEquipment = [.. itemIsScrapLookup[false]];
            List<GrabbableObject> lostItems = [];

            System.Random rng = new(StartOfRound.Instance.randomMapSeed + 197);

            bool itemsAreSafe = rng.NextDouble() < Plugin.Config.ItemsSafeChance.Value / 100;
            int maxLostScrap = System.Math.Min(Plugin.Config.MaxLostScrapItems.Value, (int)((float)itemsScrap.Count * Plugin.Config.MaxPercentLostScrapItems.Value / 100f));
            int maxLostEquipment = System.Math.Min(Plugin.Config.MaxLostEquipmentItems.Value, (int)((float)itemsEquipment.Count * Plugin.Config.MaxPercentLostEquipmentItems.Value / 100f));

            if (itemsAreSafe) { Plugin.Log.LogDebug("All items are safe!"); }
            else
            {
                itemsScrap.RemoveAll(item => !item.IsSpawned);
                int totalScrapValue = itemsScrap.Sum(scrap => scrap.scrapValue);
                int scrapLost = 0;
                int scrapValueLost = 0;

                if (Plugin.Config.ValueLossEnabled.Value)
                {
                    itemsScrap = [.. itemsScrap.OrderByDescending(scrap => scrap.scrapValue)];
                    int valueToLose = (int)(totalScrapValue * Plugin.Config.ValueLossPercent.Value / 100);
                    foreach (GrabbableObject scrap in itemsScrap)
                    {
                        if (scrapValueLost >= valueToLose || scrapLost >= maxLostScrap) break;
                        scrapValueLost += scrap.scrapValue;
                        scrapLost++;
                        lostItems.Add(scrap);
                        Plugin.Log.LogDebug($"Lost a {scrap.name} due to Value Loss");
                    }
                    itemsScrap.RemoveAll(item => !item.IsSpawned);
                    Plugin.Log.LogDebug($"Value Loss: {scrapValueLost}$ of scrap lost");
                }

                foreach (GrabbableObject scrap in itemsScrap)
                {
                    if (rng.NextDouble() < Plugin.Config.LoseEachScrapChance.Value / 100)
                    {
                        if (scrapLost >= maxLostScrap)
                        {
                            Plugin.Log.LogDebug($"Reached the maximum for lost scrap items: {maxLostScrap}");
                            break;
                        }
                        scrapValueLost += scrap.scrapValue;
                        scrapLost++;
                        lostItems.Add(scrap);
                        Plugin.Log.LogDebug($"Lost a {scrap.name} due to random chance");
                    }
                }

                Plugin.Log.LogInfo($"Lost {scrapLost} scrap items worth {scrapValueLost}");

                if (Plugin.Config.EquipmentLossEnabled.Value)
                {
                    itemsEquipment.RemoveAll(item => !item.IsSpawned);
                    int equipmentLost = 0;
                    foreach (GrabbableObject equipment in itemsEquipment)
                    {
                        if (rng.NextDouble() < Plugin.Config.LoseEachEquipmentChance.Value / 100)
                        {
                            equipmentLost++;
                            if (equipmentLost > maxLostEquipment)
                            {
                                Plugin.Log.LogDebug($"Reached the maximum for lost equipment items: {maxLostEquipment}");
                                break;
                            }
                            lostItems.Add(equipment);
                            Plugin.Log.LogDebug($"Lost a {equipment.name} to random chance");
                        }
                    }
                    Plugin.Log.LogDebug($"Lost {equipmentLost} equipment items");
                }
            }

            return lostItems;
        }
    }
}
