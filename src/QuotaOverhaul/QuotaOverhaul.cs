using Discord;
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

        public static LNetworkVariable<int> profitQuota = LNetworkVariable<int>.Connect(nameof(profitQuota), onValueChanged: SyncProfitQuota);

        public static void SetProftQuota(int value)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            profitQuota.Value = value;
        }

        static void SyncProfitQuota(int oldValue, int newValue)
        {
            TimeOfDay.Instance.profitQuota = newValue;
            Plugin.Log.LogInfo($"Synced Profit Quota: {newValue}");
        }

        public static int CalculateProfitQuota(bool usePlayerCountMultiplier = true, bool usePenaltyMultiplier = true)
        {
            int result = baseProfitQuota;
            if (Config.quotaEnablePlayerMultiplier && usePlayerCountMultiplier) result = (int)(result * quotaPlayerMultiplier);
            if (Config.quotaPenaltiesEnabled && usePenaltyMultiplier) result = (int)(result * quotaPenaltyMultiplier);
            return result;
        }

        public static void UpdatePlayerCountMultiplier()
        {
            if (!Config.quotaEnablePlayerMultiplier.Value)
            {
                return;
            }
            quotaPlayerMultiplier = CalculatePlayerCountMultiplier();
            SetProftQuota(CalculateProfitQuota());

            Plugin.Log.LogInfo($"Player Count Multiplier: {quotaPlayerMultiplier}");
        }
        
        public static double CalculatePlayerCountMultiplier()
        {
            int playersCounted = recordPlayersThisQuota;
            playersCounted = math.clamp(recordPlayersThisQuota, Config.quotaPlayerThreshold.Value, Config.quotaPlayerCap.Value);
            playersCounted -= Config.quotaPlayerThreshold.Value;
            return Config.quotaMultPerPlayer.Value * math.max(playersCounted, 0);
        }

        public static void OnNewSession()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            var quotaVariables = TimeOfDay.Instance.quotaVariables;

            quotaVariables.startingQuota = Config.startingQuota.Value;
            quotaVariables.baseIncrease = Config.quotaBaseIncrease.Value;
            quotaVariables.increaseSteepness = Config.quotaIncreaseSteepness.Value;
            quotaVariables.randomizerMultiplier = Config.quotaRandomizerMultiplier.Value;
            quotaVariables.startingCredits = Config.startingCredits;
            quotaVariables.deadlineDaysAmount = Config.quotaDeadline;

            TimeOfDay.Instance.quotaVariables = quotaVariables;
            Plugin.Log.LogInfo("Quota Variables Configured");

            OnNewRun();
        }

        public static void OnNewRun()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            baseProfitQuota = TimeOfDay.Instance.quotaVariables.startingQuota;
            OnNewQuota();
        }

        public static void OnNewQuota()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            quotaInProgress = false;
            quotaPenaltyMultiplier = 1;
            recordPlayersThisQuota = StartOfRound.Instance.connectedPlayersAmount;
            SetProftQuota(CalculateProfitQuota());
        }

        public static void OnPlayerCountChanged()
        {
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