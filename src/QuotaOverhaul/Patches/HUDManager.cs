using HarmonyLib;

namespace QuotaOverhaul.Patches
{
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))]
    public class DeathPenaltyPatch
    {
        public static bool Prefix()
        {
            return false;
        }

        public static void Postfix(int playersDead, int bodiesInsured)
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            
            int oldCredits = terminal.groupCredits;
            bool doCreditPenalty = Config.CreditPenaltiesEnabled.Value && (Config.CreditPenaltiesOnGordion.Value || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion");
            double creditPenalty = 0d;
            
            if (doCreditPenalty)
            {
                creditPenalty = Config.CreditPenaltiesDynamic.Value ? CalculateDynamicCreditPenalty(playersDead, bodiesInsured) : CalculateCreditPenalty(playersDead, bodiesInsured);

                if (GameNetworkManager.Instance.isHostingGame)
                {
                    terminal.groupCredits -= (int)(oldCredits * creditPenalty);
                    if (terminal.groupCredits < 0)
                    {
                        terminal.groupCredits = 0;
                    }
                }
            }

            int oldQuota = TimeOfDay.Instance.profitQuota;
            bool doQuotaPenalty = Config.QuotaPenaltiesEnabled.Value && (Config.QuotaPenaltiesOnGordion.Value || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion");
            double quotaPenalty = 0d;
            
            if (doQuotaPenalty)
            {
                quotaPenalty = Config.QuotaPenaltiesDynamic.Value ? CalculateDynamicQuotaPenalty(playersDead, bodiesInsured) : CalculateQuotaPenalty(playersDead, bodiesInsured);

                if (GameNetworkManager.Instance.isHostingGame)
                {
                    QuotaOverhaul.QuotaPenaltyMultiplier += quotaPenalty;
                    QuotaOverhaul.ProfitQuota.Value = QuotaOverhaul.CalculateProfitQuota();
                }
            }
            
            string penaltyAdditionText = $"CASUALTIES: ${playersDead}\nBODIES RECOVERED: ${bodiesInsured} \n \nCREDITS: -{(int)(creditPenalty * 100)}% \n${oldCredits} -> ${terminal.groupCredits} \n \nQUOTA: +{(int)(quotaPenalty * 100)}% \n${oldQuota} -> ${TimeOfDay.Instance.profitQuota}";
            
            HUDManager.Instance.statsUIElements.penaltyAddition.text = penaltyAdditionText;
            HUDManager.Instance.statsUIElements.penaltyTotal.text = "";

            
        }

        private static double CalculateCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Config.CreditPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.CreditPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Credit Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.RecordPlayersThisMoon * Config.CreditPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.CreditPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.CreditPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Credit Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Config.QuotaPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.QuotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Quota Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            Plugin.Log.LogInfo($"Calculating Dynamic Quota Penalty");
            double penaltyPerBody = 1d / QuotaOverhaul.RecordPlayersThisMoon * Config.QuotaPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.QuotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Penalty fell below threshold of {Config.QuotaPenaltyPercentThreshold.Value / 100d}");
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");
            return penalty;
        }
    }
}