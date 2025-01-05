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
        }

        public static float ApplyQuotaPenalty(int recoveredBodies, int unrecoveredBodies)
        {
            if (!Config.quotaPenaltiesEnabled.Value)
            {
                return 0;
            }
            float unrecoveredBodyPenalty = 1 / recordPlayersThisMoon;
            float recoveredBodyPenalty = 1 / recordPlayersThisMoon * (100 - Config.bodyRecoveryBonus.Value) / 100;
            float penalty = (unrecoveredBodies * unrecoveredBodyPenalty + recoveredBodies * recoveredBodyPenalty) * (Config.penaltyMaxPercent.Value / 100);
            if (penalty < Config.penaltyPercentThreshold.Value)
            {
                penalty = 0;
            }
            penaltyMultiplier += penalty;
            return penalty;
        }
    }
}