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
            QuotaOverhaul.QuotaPenaltyMultiplier.Increase(quotaPenalty);

            string penaltyAdditionText = $"CASUALTIES: {deadBodies}\nBODIES RECOVERED: {recoveredBodies} \n \nCREDITS: -{(int)(creditPenalty * 100)}% \n{oldCredits} -> {terminal.groupCredits} \n \nQUOTA: +{(int)(quotaPenalty * 100)}% \n{oldQuota} -> {TimeOfDay.Instance.profitQuota}";

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
            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");

            if (penalty < 0 || penalty < Plugin.Config.QuotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Penalty fell below threshold of {Plugin.Config.QuotaPenaltyPercentThreshold.Value / 100d}");
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

            if (!itemsAreSafe)
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
                        if (scrapValueLost >= valueToLose || scrapLost >= Plugin.Config.MaxLostScrapItems.Value) break;
                        scrapValueLost += scrap.scrapValue;
                        scrapLost++;
                        lostItems.Add(scrap);
                    }
                    itemsScrap.RemoveAll(item => !item.IsSpawned);
                    Plugin.Log.LogInfo($"Value Loss: {scrapValueLost}$ of scrap lost");
                }

                foreach (GrabbableObject scrap in itemsScrap)
                {
                    if (rng.NextDouble() < Plugin.Config.LoseEachScrapChance.Value / 100)
                    {
                        if (scrapLost >= Plugin.Config.MaxLostScrapItems.Value) break;
                        scrapValueLost += scrap.scrapValue;
                        scrapLost++;
                        lostItems.Add(scrap);
                    }
                }

                Plugin.Log.LogInfo($"Lost {scrapLost} scrap items worth {scrapValueLost}");
            }

            if (!Plugin.Config.EquipmentLossEnabled.Value)
            {
                Plugin.Log.LogInfo("Equipment loss is disabled");
            }
            else if (!itemsAreSafe)
            {
                itemsEquipment.RemoveAll(item => !item.IsSpawned);
                int equipmentLost = 0;
                foreach (GrabbableObject equipment in itemsEquipment)
                {
                    if (rng.NextDouble() < Plugin.Config.LoseEachEquipmentChance.Value / 100)
                    {
                        equipmentLost++;
                        if (equipmentLost > Plugin.Config.MaxLostEquipmentItems.Value) break;
                        lostItems.Add(equipment);
                    }
                }
                Plugin.Log.LogInfo($"Lost {equipmentLost} equipment items");
            }

            return lostItems;
        }
    }
}
