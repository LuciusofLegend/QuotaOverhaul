using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace QuotaOverhaul
{
    public class DeathConsequences
    {

        public static void DoDeathConsequences(int deadBodies, int recoveredBodies)
        {
            Plugin.Log.LogInfo($"Calculating Death Consequences for {QuotaOverhaul.GetRecordPlayersThisMoon()} players.  {deadBodies} dead, {recoveredBodies} recovered.");
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

            int oldCredits = terminal.groupCredits;
            double creditPenaltyFraction = CalculateCreditPenalty(deadBodies, recoveredBodies);
            int creditPenaltyTotal = (int)(oldCredits * creditPenaltyFraction);
            Plugin.Log.LogInfo($"You lost {creditPenaltyTotal} of {terminal.groupCredits} credits");

            if (GameNetworkManager.Instance.isHostingGame)
            {
                terminal.groupCredits -= creditPenaltyTotal;
                if (terminal.groupCredits < 0) terminal.groupCredits = 0;
            }

            int oldQuota = TimeOfDay.Instance.profitQuota;
            double quotaPenalty = 0d;

            quotaPenalty = CalculateQuotaPenalty(deadBodies, recoveredBodies);

            int creditPenaltyFromCombinedSystem = 0;
            if (Plugin.Config.ChargeCreditsInsteadOfQuota.Value)
            {
                Plugin.Log.LogInfo("Applying combined penalty system...");
                int creditsOwed = (int)(TimeOfDay.Instance.profitQuota * (double)Plugin.Config.CreditsPerQuota.Value * quotaPenalty);
                Plugin.Log.LogInfo($"Quota: {TimeOfDay.Instance.profitQuota}, Credits per Quota: {Plugin.Config.CreditsPerQuota.Value}, Multiply: {TimeOfDay.Instance.profitQuota * Plugin.Config.CreditsPerQuota.Value}, Quota Penalty: {quotaPenalty} \nTotal: {creditsOwed}");
                //Plugin.Log.LogInfo($"You owe {creditsOwed} credits");
                if (terminal.groupCredits >= creditsOwed)
                {
                    Plugin.Log.LogInfo($"You have {terminal.groupCredits} credits, which is enough to cover the penalty");
                    creditPenaltyFromCombinedSystem = creditsOwed;
                    quotaPenalty = 0;
                }
                else
                {
                    Plugin.Log.LogInfo($"You have {terminal.groupCredits} credits, which is NOT enough to cover the penalty");
                    creditPenaltyFromCombinedSystem = terminal.groupCredits;
                    quotaPenalty = 1 / creditsOwed * terminal.groupCredits * quotaPenalty / (double)Plugin.Config.CreditsPerQuota.Value;
                    Plugin.Log.LogInfo($"You've lost all your credits and the remaining quota penalty is {quotaPenalty}");
                }
                terminal.groupCredits -= creditPenaltyFromCombinedSystem;
                if (terminal.groupCredits < 0) terminal.groupCredits = 0;
            }
            QuotaOverhaul.QuotaPenaltyMultiplier.Increase(quotaPenalty);

            string penaltyAdditionText = $"CASUALTIES: {deadBodies}\nBODIES RECOVERED: {recoveredBodies} \n \nCREDITS: -{(int)(creditPenaltyFraction * 100)}% -{creditPenaltyFromCombinedSystem} \n{oldCredits} -> {terminal.groupCredits} \n \nQUOTA: +{(int)(quotaPenalty * 100)}% \n{oldQuota} -> {TimeOfDay.Instance.profitQuota}";
            HUDManager.Instance.statsUIElements.penaltyAddition.text = penaltyAdditionText;
            HUDManager.Instance.statsUIElements.penaltyTotal.text = "";
        }

        public static double CalculateCreditPenalty(int deadBodies, int recoveredBodies)
        {
            if (!Plugin.Config.CreditPenaltiesEnabled.Value)
            {
                Plugin.Log.LogInfo("Credit Penalties Disabled");
                return 0;
            }
            if (!Plugin.Config.CreditPenaltiesOnGordion.Value && StartOfRound.Instance.currentLevel.PlanetName == "71 Gordion")
            {
                Plugin.Log.LogInfo("Credit Penalties Disabled on Gordion");
                return 0;
            }

            double penalty = 0;
            double penaltyThreshold = Plugin.Config.CreditPenaltyThreshold.Value;
            if (Plugin.Config.CreditPenaltiesDynamic.Value) penalty = CalculateDynamicCreditPenalty(deadBodies, recoveredBodies);
            else penalty = CalculateStaticCreditPenalty(deadBodies, recoveredBodies);

            if (penalty < 0 || penalty < penaltyThreshold)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Credit Penalty fell below the threshold of {penaltyThreshold}.  No penalty will be applied.");
            }
            return penalty;
        }

        private static double CalculateStaticCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Plugin.Config.CreditPenaltyPerPlayer.Value;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.QuotaPenaltyRecoveryBonus.Value;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            Plugin.Log.LogInfo($"Calculated Credit Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.GetRecordPlayersThisMoon() * Plugin.Config.CreditPenaltyCap.Value;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.CreditPenaltyRecoveryBonus.Value;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            Plugin.Log.LogInfo($"Calculated Dynamic Credit Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateTeamWipeCreditPenalty(int recoveredBodies)
        {
            int playerCount = QuotaOverhaul.GetRecordPlayersThisMoon();
            double penaltyPerBody = 1d / playerCount * Plugin.Config.CreditPenaltyOnTeamWipe.Value;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.CreditPenaltyRecoveryBonus.Value;
            double penalty = playerCount * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            Plugin.Log.LogInfo($"Calculated Team Wipe Credit Penalty of {penalty}");
            return penalty;
        }

        public static double CalculateQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            if (!Plugin.Config.QuotaPenaltiesEnabled.Value)
            {
                Plugin.Log.LogInfo("Quota Penalties Disabled");
                return 0;
            }
            if (!Plugin.Config.QuotaPenaltiesOnGordion.Value && StartOfRound.Instance.currentLevel.PlanetName == "71 Gordion")
            {
                Plugin.Log.LogInfo("Quota Penalties Disabled on Gordion");
                return 0;
            }

            double penalty = 0;
            double penaltyThreshold = Plugin.Config.QuotaPenaltyThreshold.Value;
            if (Plugin.Config.QuotaPenaltiesDynamic.Value) penalty = CalculateDynamicQuotaPenalty(deadBodies, recoveredBodies);
            else penalty = CalculateStaticQuotaPenalty(deadBodies, recoveredBodies);

            if (penalty < 0 || penalty < penaltyThreshold)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Quota Penalty fell below the threshold of {penaltyThreshold}.  No penalty will be applied.");
            }
            return penalty;
        }

        private static double CalculateStaticQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Plugin.Config.QuotaPenaltyPerPlayer.Value;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.QuotaPenaltyRecoveryBonus.Value;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;
            Plugin.Log.LogInfo($"Calculated Quota Penalty of {penalty}");

            if (penalty < 0 || penalty < Plugin.Config.QuotaPenaltyThreshold.Value)
            {
                Plugin.Log.LogInfo($"Quota penalty fell below threshold of {Plugin.Config.QuotaPenaltyThreshold.Value}.  No penalty will be applied.");
                penalty = 0;
            }

            return penalty;
        }

        private static double CalculateDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.GetRecordPlayersThisMoon() * Plugin.Config.QuotaPenaltyCap.Value;
            double bonusPerRecoveredBody = penaltyPerBody * Plugin.Config.QuotaPenaltyRecoveryBonus.Value;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;
            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");

            if (penalty < 0 || penalty < Plugin.Config.QuotaPenaltyThreshold.Value)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Penalty fell below threshold of {Plugin.Config.QuotaPenaltyThreshold.Value}.  No penalty will be applied.");
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

            bool itemsAreSafe = rng.NextDouble() < Plugin.Config.ItemsSafeChance.Value;
            int maxLostScrap = System.Math.Min(Plugin.Config.MaxLostScrapItems.Value, (int)((float)itemsScrap.Count * Plugin.Config.MaxFractionLostScrapItems.Value));
            int maxLostEquipment = System.Math.Min(Plugin.Config.MaxLostEquipmentItems.Value, (int)((float)itemsEquipment.Count * Plugin.Config.MaxFractionLostEquipmentItems.Value));

            if (itemsAreSafe) { Plugin.Log.LogInfo("All items are safe!"); }
            else
            {
                if (Plugin.Config.ScrapLossEnabled.Value)
                {
                    Plugin.Log.LogInfo("Scrap loss is enabled");
                    itemsScrap.RemoveAll(item => !item.IsSpawned);
                    int totalScrapValue = itemsScrap.Sum(scrap => scrap.scrapValue);
                    int scrapLost = 0;
                    int scrapValueLost = 0;

                    if (Plugin.Config.ValueLossEnabled.Value)
                    {
                        itemsScrap = [.. itemsScrap.OrderByDescending(scrap => scrap.scrapValue)];
                        int valueToLose = (int)(totalScrapValue * Plugin.Config.ValueLossAmount.Value);
                        foreach (GrabbableObject scrap in itemsScrap)
                        {
                            if (scrapValueLost >= valueToLose || scrapLost >= maxLostScrap) break;
                            scrapValueLost += scrap.scrapValue;
                            scrapLost++;
                            lostItems.Add(scrap);
                            Plugin.Log.LogInfo($"Lost a {scrap.name} due to Value Loss");
                        }
                        itemsScrap.RemoveAll(item => !item.IsSpawned);
                        Plugin.Log.LogInfo($"Value Loss: {scrapValueLost}$ of scrap lost");
                    }

                    foreach (GrabbableObject scrap in itemsScrap)
                    {
                        if (rng.NextDouble() < Plugin.Config.LoseEachScrapChance.Value)
                        {
                            if (scrapLost >= maxLostScrap)
                            {
                                Plugin.Log.LogInfo($"Reached the maximum for lost scrap items: {maxLostScrap}");
                                break;
                            }
                            scrapValueLost += scrap.scrapValue;
                            scrapLost++;
                            lostItems.Add(scrap);
                            Plugin.Log.LogInfo($"Lost a {scrap.name} due to random chance");
                        }
                    }

                    Plugin.Log.LogInfo($"Lost {scrapLost} scrap items worth {scrapValueLost}");
                }
                else Plugin.Log.LogInfo("Scrap loss is disabled");

                if (Plugin.Config.EquipmentLossEnabled.Value)
                {
                    Plugin.Log.LogInfo("Equipment loss is enabled");
                    itemsEquipment.RemoveAll(item => !item.IsSpawned);
                    int equipmentLost = 0;
                    foreach (GrabbableObject equipment in itemsEquipment)
                    {
                        if (rng.NextDouble() < Plugin.Config.LoseEachEquipmentChance.Value)
                        {
                            equipmentLost++;
                            if (equipmentLost >= maxLostEquipment)
                            {
                                Plugin.Log.LogInfo($"Reached the maximum for lost equipment items: {maxLostEquipment}");
                                break;
                            }
                            lostItems.Add(equipment);
                            Plugin.Log.LogInfo($"Lost a {equipment.name} to random chance");
                        }
                    }
                    Plugin.Log.LogInfo($"Lost {equipmentLost} equipment items");
                }
                else Plugin.Log.LogInfo("Equipment loss is disabled");
            }

            return lostItems;
        }
    }
}
