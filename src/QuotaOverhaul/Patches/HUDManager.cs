using System.Runtime.CompilerServices;
using HarmonyLib;

namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))]
    public class DeathPenaltyPatch
    {
        [HarmonyPrefix]
        public static void SkipDefaultDeathPenalty()
        {
            return;
        }

        [HarmonyPostfix]
        public static void CustomDeathPenalty(int playersDead, int bodiesInsured)
        {
            if (Config.creditPenaltiesEnabled.Value)
            {
                float creditPenalty;
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
            }

            if (Config.quotaPenaltiesEnabled.Value)
            {
                float quotaPenalty;
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
            }
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

            if (penalty < 0 || penalty < Config.quotaPenaltyPercentThreshold.Value)
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

            if (penalty < 0 || penalty < Config.quotaPenaltyPercentThreshold.Value)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");
            return penalty;
        }
    }
}