using HarmonyLib;

namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))]
    public class DeathPenaltyPatch
    {
        [HarmonyPrefix]
        public static bool SkipDefaultDeathPenalty()
        {
            return false;
        }

        [HarmonyPostfix]
        public static void CustomDeathPenalty(int playersDead, int bodiesInsured)
        {
            float creditPenalty;
            float quotaPenalty;
            int oldQuota = TimeOfDay.Instance.profitQuota;
            string penaltyAdditionText = $"{playersDead} casualties | {bodiesInsured} bodies recovered";
            string penaltyTotalText = "";
            if (Config.creditPenaltiesEnabled.Value)
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
                terminal.groupCredits -= (int)(credits * creditPenalty);
                if (terminal.groupCredits < 0)
                {
                    terminal.groupCredits = 0;
                }

                penaltyAdditionText += $"\nCREDITS: -{(int)(creditPenalty * 100)}%";
                penaltyTotalText += $"\ncharged {(int)(credits * creditPenalty)} credits";
            }

            if (Config.quotaPenaltiesEnabled.Value)
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
                QuotaOverhaul.UpdateProfitQuota();

                penaltyAdditionText += $"\nQUOTA: +{(int)(quotaPenalty * 100)}%";
                penaltyTotalText += $"\nraised quota by {TimeOfDay.Instance.profitQuota - oldQuota}";
            }
            HUDManager.Instance.statsUIElements.penaltyAddition.text = penaltyAdditionText;
            HUDManager.Instance.statsUIElements.penaltyTotal.text = penaltyTotalText;
        }

        public static float CalculateCreditPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = Config.creditPenaltyPercentPerPlayer.Value / 100;
            float bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100;
            float penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Credit Penalty of {penalty}");
            return penalty;
        }

        public static float CalculateDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = 1 / QuotaOverhaul.recordPlayersThisMoon;
            float bonusPerRecoveredBody = penaltyPerBody * Config.creditPenaltyRecoveryBonus.Value / 100;
            float penalty = (deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody) * (Config.creditPenaltyPercentCap.Value / 100);

            if (penalty < 0 || penalty < Config.quotaPenaltyPercentThreshold.Value / 100)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Credit Penalty of {penalty}");
            return penalty;
        }

        public static float CalculateQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = Config.quotaPenaltyPercentPerPlayer.Value / 100;
            float bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100;
            float penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Quota Penalty of {QuotaOverhaul.quotaPlayerMultiplier}");
            return penalty;
        }

        public static float CalculateDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = 1 / QuotaOverhaul.recordPlayersThisMoon;
            float bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100;
            float penalty = (deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody) * (Config.quotaPenaltyPercentCap.Value / 100);

            if (penalty < 0 || penalty < Config.quotaPenaltyPercentThreshold.Value / 100)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");
            return penalty;
        }
    }
}