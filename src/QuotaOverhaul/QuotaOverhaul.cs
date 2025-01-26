using Unity.Mathematics;

namespace QuotaOverhaul
{
    public static class QuotaOverhaul
    {
        public static int baseProfitQuota = 0;
        public static float quotaPenaltyMultiplier = 1;
        public static float quotaPlayerMultiplier = 1;

        public static int recordPlayersThisQuota = 0;
        public static int recordPlayersThisMoon = 0;

        public static void UpdateProfitQuota()
        {
            TimeOfDay.Instance.profitQuota = (int)(baseProfitQuota * quotaPlayerMultiplier * quotaPenaltyMultiplier);

            Plugin.Log.LogInfo($"Quota Update: {quotaPlayerMultiplier}");
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
            quotaPlayerMultiplier = FindPlayerCountMultiplier();
            UpdateProfitQuota();

            Plugin.Log.LogInfo($"New Player Multiplier: {quotaPlayerMultiplier}");
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