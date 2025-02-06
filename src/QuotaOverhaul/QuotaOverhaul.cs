using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;
using LethalNetworkAPI;
using Unity.Mathematics;

namespace QuotaOverhaul
{
    public class QuotaOverhaul
    {
        [ModData(SaveWhen.OnSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
        public static int baseProfitQuota = 0;
        [ModData(SaveWhen.OnSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
        public static double quotaPenaltyMultiplier = 1;
        public static double quotaPlayerMultiplier = 1;

        [ModData(SaveWhen.OnSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
        public static int recordPlayersThisQuota = 1;
        [ModData(SaveWhen.OnSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
        public static int recordPlayersThisMoon = 1; 

        [ModData(SaveWhen.OnSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
        public static bool quotaInProgress = false;

        [ModData(SaveWhen.OnSave, LoadWhen.OnLoad, SaveLocation.CurrentSave)]
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
    }
}