using LethalNetworkAPI;
using Unity.Mathematics;

namespace QuotaOverhaul
{
    public class QuotaOverhaul
    {
        public static int baseProfitQuota = 0;
        public static double quotaPenaltyMultiplier = 1;
        public static double quotaPlayerMultiplier = 1;
        public static int recordPlayersThisQuota = 1;
        public static int recordPlayersThisMoon = 1;
        public static bool quotaInProgress = false;

        public static LNetworkVariable<int> profitQuota = LNetworkVariable<int>.Connect("profitQuota", onValueChanged: SyncProfitQuota);

        static void SyncProfitQuota(int oldValue, int newValue)
        {
            TimeOfDay.Instance.profitQuota = newValue;
            Plugin.Log.LogInfo($"Profit Quota updated to {newValue}");
        }

        public static void UpdateProfitQuota()
        {
            profitQuota.Value = (int)(baseProfitQuota * quotaPlayerMultiplier * quotaPenaltyMultiplier);
        }

        public static double FindPlayerCountMultiplier()
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

        public static void OnPlayerCountChanged()
        {
            Plugin.Log.LogInfo("Custom OnPlayerConnect() called");

            int playerCount = StartOfRound.Instance.connectedPlayersAmount + 1;
            if (!StartOfRound.Instance.shipHasLanded)
            {
                recordPlayersThisMoon = playerCount;
            }
            else if (playerCount > recordPlayersThisMoon)
            {
                recordPlayersThisMoon = playerCount;
                Plugin.Log.LogInfo($"Record Players this Moon: {recordPlayersThisMoon}");
            }
            if (!quotaInProgress)
            {
                recordPlayersThisQuota = playerCount;
            }
            else if (playerCount > recordPlayersThisQuota)
            {
                recordPlayersThisQuota = playerCount;
                UpdatePlayerCountMultiplier();
                Plugin.Log.LogInfo($"Record Players this Quota: {recordPlayersThisQuota}");
            }
        }

        public static void LoadData()
        {
            string saveFile = GameNetworkManager.Instance.currentSaveFileName;
            if (ES3.KeyExists(nameof(baseProfitQuota), saveFile)) baseProfitQuota = ES3.Load<int>(nameof(baseProfitQuota), saveFile);
            else baseProfitQuota = Config.startingQuota;
            if (ES3.KeyExists(nameof(quotaPenaltyMultiplier), saveFile)) quotaPenaltyMultiplier = ES3.Load<double>(nameof(quotaPenaltyMultiplier), saveFile);
            if (ES3.KeyExists(nameof(recordPlayersThisQuota), saveFile)) recordPlayersThisQuota =  ES3.Load<int>(nameof(recordPlayersThisQuota), saveFile);
            if (ES3.KeyExists(nameof(quotaInProgress), saveFile)) quotaInProgress = ES3.Load<bool>(nameof(quotaInProgress), saveFile);
        }

        public static void SaveData()
        {
            string saveFile = GameNetworkManager.Instance.currentSaveFileName;
            ES3.Save<int>(nameof(baseProfitQuota), baseProfitQuota, saveFile);
            ES3.Save<double>(nameof(quotaPenaltyMultiplier), quotaPenaltyMultiplier, saveFile);
            ES3.Save<int>(nameof(recordPlayersThisQuota), recordPlayersThisQuota, saveFile);
            ES3.Save<bool>(nameof(quotaInProgress), quotaInProgress, saveFile);
        }
    }
}