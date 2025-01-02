using System;

namespace QuotaOverhaul
{
    public static class QuotaManager
    {
        public static int baseProfitQuota = 0;
        public static int quotaPenalties = 0;
        public static float quotaMultiplier = 1;

        public static void UpdateProfitQuota()
        {
            TimeOfDay.Instance.profitQuota = (int)((baseProfitQuota + quotaPenalties) * quotaMultiplier);
        }

        public static void UpdateQuotaMultiplier()
        {
            quotaMultiplier = 1 + Math.Max(StartOfRound.Instance.connectedPlayersAmount - Config.quotaPlayerThreshold.Value, 0) * Config.quotaMultPerPlayer.Value;
            UpdateProfitQuota();
        }
    }
}