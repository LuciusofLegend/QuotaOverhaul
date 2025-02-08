using HarmonyLib;

namespace QuotaOverhaul
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
            bool doCreditPenalty = Config.creditPenaltiesEnabled && (Config.creditPenaltiesOnGordion || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion");
            double creditPenalty;

            bool doQuotaPenalty = Config.quotaPenaltiesEnabled && (Config.quotaPenaltiesOnGordion || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion");
            double quotaPenalty;

            int oldQuota = TimeOfDay.Instance.profitQuota;

            string penaltyAdditionText = $"{playersDead} casualties | {bodiesInsured} bodies recovered";
            string penaltyTotalText = "";
            
            if (doCreditPenalty)
            {
                if (Config.creditPenaltiesDynamic.Value)
                {
                    creditPenalty = CalculateDynamicCreditPenalty(playersDead, bodiesInsured);
                }
                else
                {
                    creditPenalty = CalculateCreditPenalty(playersDead, bodiesInsured);
                }

                Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                int credits = terminal.groupCredits;

                if (GameNetworkManager.Instance.isHostingGame)
                {
                    terminal.groupCredits -= (int)(credits * creditPenalty);
                    if (terminal.groupCredits < 0)
                    {
                        terminal.groupCredits = 0;
                    }
                }

                penaltyAdditionText += $"\nCREDITS: -{(int)(creditPenalty * 100)}%";
                penaltyTotalText += $"\ncharged {(int)(credits * creditPenalty)} credits";
            }

            if (doQuotaPenalty)
            {
                if (Config.quotaPenaltiesDynamic.Value)
                {
                    quotaPenalty = CalculateDynamicQuotaPenalty(playersDead, bodiesInsured);
                }
                else
                {
                    quotaPenalty = CalculateQuotaPenalty(playersDead, bodiesInsured);
                }

                QuotaOverhaul.quotaPenaltyMultiplier += quotaPenalty;
                QuotaOverhaul.SetProftQuota(QuotaOverhaul.CalculateProfitQuota());

                penaltyAdditionText += $"\nQUOTA: +{(int)(quotaPenalty * 100)}%";
                penaltyTotalText += $"\nraised quota by {TimeOfDay.Instance.profitQuota - oldQuota}";
            }
            HUDManager.Instance.statsUIElements.penaltyAddition.text = penaltyAdditionText;
            HUDManager.Instance.statsUIElements.penaltyTotal.text = penaltyTotalText;
        }

        public static double CalculateCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Config.creditPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Credit Penalty of {penalty}");
            return penalty;
        }

        public static double CalculateDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.recordPlayersThisMoon * Config.creditPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.creditPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.quotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Credit Penalty of {penalty}");
            return penalty;
        }

        public static double CalculateQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Config.quotaPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Quota Penalty of {penalty}");
            return penalty;
        }

        public static double CalculateDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            Plugin.Log.LogInfo($"Calculaing Dynamic Quota Penalty");
            double penaltyPerBody = 1d / QuotaOverhaul.recordPlayersThisMoon * Config.quotaPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.quotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Penalty fell below threshold of {Config.quotaPenaltyPercentThreshold.Value / 100d}");
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");
            return penalty;
        }
    }
}