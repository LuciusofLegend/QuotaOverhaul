using Unity.Mathematics;

namespace QuotaOverhaul
{
    public static class QuotaManager
    {
        public static int baseProfitQuota = 0;
        public static float penaltyMultiplier = 1;
        public static float playerCountMultiplier = 1;

        public static int recordPlayersThisQuota = 0;
        public static int recordPlayersThisMoon = 0;

        public static void UpdateProfitQuota()
        {
            TimeOfDay.Instance.profitQuota = (int)(baseProfitQuota * playerCountMultiplier * penaltyMultiplier);

            Plugin.Log.LogInfo($"Quota Update: {playerCountMultiplier}");
        }

        public static float FindPlayerCountMultiplier()
        {
            return Config.quotaMultPerPlayer.Value * math.max(math.clamp(recordPlayersThisQuota, Config.quotaPlayerThreshold.Value, Config.quotaPlayerCap.Value) - Config.quotaPlayerThreshold.Value, 0);
        }

        public static void UpdatePlayerCountMultiplier()
        {
            if (!Config.quotaEnablePlayerMultiplier.Value)
            {
                return;
            }
            playerCountMultiplier = FindPlayerCountMultiplier();
            UpdateProfitQuota();

            Plugin.Log.LogInfo($"New Player Multiplier: {playerCountMultiplier}");
        }

        public static float ApplyQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = Config.quotaPenaltyPercentPerPlayer.Value / 100;
            float bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100;
            float penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;
            penaltyMultiplier += penalty;

            Plugin.Log.LogInfo($"Applied Quota Penalty of {playerCountMultiplier}");
            return penalty;
        }

        public static float ApplyDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = 1 / recordPlayersThisMoon;
            float bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100;
            float penalty = (deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody) * (Config.quotaPenaltyPercentCap.Value / 100);

            if (penalty < Config.quotaPenaltyPercentThreshold.Value)
            {
                penalty = 0;
            }

            penaltyMultiplier += penalty;
            Plugin.Log.LogInfo($"Dynamically Applied Quota Penalty of {playerCountMultiplier}");
            return penalty;
        }

        public static float ApplyCreditPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = Config.creditPenaltyPercentPerPlayer.Value / 100;
            float bonusPerRecoveredBody = penaltyPerBody * Config.quotaPenaltyRecoveryBonus.Value / 100;
            float penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            // subtract credits
            Plugin.Log.LogInfo($"Applied Credit Penalty of {playerCountMultiplier}");
            return penalty;
        }

        public static float ApplyDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            float penaltyPerBody = 1 / recordPlayersThisMoon;
            float bonusPerRecoveredBody = penaltyPerBody * Config.creditPenaltyRecoveryBonus.Value / 100;
            float penalty = (deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody) * (Config.creditPenaltyPercentCap.Value / 100);

            if (penalty < Config.creditPenaltyPercentThreshold.Value)
            {
                penalty = 0;
            }

            //subtract credits
            Plugin.Log.LogInfo($"Dynamically Applied Credit Penalty of {playerCountMultiplier}");
            return penalty;
        }

        public static void OnPlayerConnect()
        {
            Plugin.Log.LogInfo("Custom OnPlayerConnect() called");

            int playerCount = StartOfRound.Instance.connectedPlayersAmount;
            if (playerCount > recordPlayersThisMoon)
            {
                recordPlayersThisMoon = playerCount;
                Plugin.Log.LogInfo($"Record Players this Quota: {recordPlayersThisQuota}");
            }
            if (playerCount > recordPlayersThisQuota)
            {
                recordPlayersThisQuota = playerCount;
                UpdatePlayerCountMultiplier();
                Plugin.Log.LogInfo($"Record Players this Quota: {recordPlayersThisQuota}");
            }
        }
    }
}