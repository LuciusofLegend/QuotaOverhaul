using LethalNetworkAPI;
using Unity.Mathematics;

namespace QuotaOverhaul
{
    public static class QuotaOverhaul
    {
        private static int BaseProfitQuota;
        private static readonly LNetworkVariable<int> SyncedProfitQuota = LNetworkVariable<int>.Connect(nameof(SyncedProfitQuota), onValueChanged: SyncQuotaToClients);
        private static void SyncQuotaToClients(int oldValue, int newValue) { TimeOfDay.Instance.profitQuota = newValue; }
        public static readonly QuotaMultiplier QuotaPenaltyMultiplier = new QuotaMultiplier(nameof(QuotaPenaltyMultiplier), 1d);
        public static readonly QuotaMultiplier QuotaPlayerMultiplier = new QuotaMultiplier(nameof(QuotaPlayerMultiplier), 1d, false);
        private static int RecordPlayersThisQuota = 1;
        private static int RecordPlayersThisMoon = 1;
        public static bool QuotaInProgress;

        public static void UpdateProfitQuota()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            SyncedProfitQuota.Value = QuotaMultiplier.GetQuotaWithMultipliers();
        }

        public static int GetBaseProfitQuota()
        {
            return BaseProfitQuota;
        }

        public static void SetBaseProfitQuota(int value)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            BaseProfitQuota = value;
            UpdateProfitQuota();
        }

        public static int GetRecordPlayersThisQuota()
        {
            return RecordPlayersThisQuota;
        }

        public static void SetRecordPlayersThisQuota(int value)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            RecordPlayersThisQuota = value;
            QuotaPlayerMultiplier.Set(CalculatePlayerCountMultiplier());
        }

        public static double CalculatePlayerCountMultiplier()
        {
            int playersCounted = math.clamp(RecordPlayersThisQuota, Plugin.Config.QuotaPlayerThreshold.Value, Plugin.Config.QuotaPlayerCap.Value);
            playersCounted -= Plugin.Config.QuotaPlayerThreshold.Value;
            double result = 1 + Plugin.Config.QuotaMultiplierPerPlayer.Value * math.max(playersCounted, 0);
            Plugin.Log.LogDebug("Calculated player count multiplier of " + result);
            return result;
        }

        public static int GetRecordPlayersThisMoon()
        {
            return RecordPlayersThisMoon;
        }

        public static void SetRecordPlayersThisMoon(int value)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            RecordPlayersThisMoon = value;
        }

        public static void OnNewSession()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            if (Plugin.Config.StartingQuota.Value != 130) TimeOfDay.Instance.quotaVariables.startingQuota = Plugin.Config.StartingQuota.Value;
            if (Plugin.Config.QuotaBaseIncrease.Value != 200) TimeOfDay.Instance.quotaVariables.baseIncrease = Plugin.Config.QuotaBaseIncrease.Value;
            if (Plugin.Config.QuotaIncreaseSteepness.Value != 4f) TimeOfDay.Instance.quotaVariables.increaseSteepness = Plugin.Config.QuotaIncreaseSteepness.Value;
            if (Plugin.Config.QuotaRandomizerMultiplier.Value != 1f) TimeOfDay.Instance.quotaVariables.randomizerMultiplier = Plugin.Config.QuotaRandomizerMultiplier.Value;
            if (Plugin.Config.StartingCredits.Value != 60) TimeOfDay.Instance.quotaVariables.startingCredits = Plugin.Config.StartingCredits.Value;
            if (Plugin.Config.QuotaDeadline.Value != 3) TimeOfDay.Instance.quotaVariables.deadlineDaysAmount = Plugin.Config.QuotaDeadline.Value;

            LoadData();

            if (TimeOfDay.Instance.timesFulfilledQuota == 0 && !QuotaInProgress) OnNewRun();
        }

        public static void OnNewRun()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            SetBaseProfitQuota(TimeOfDay.Instance.quotaVariables.startingQuota);
            OnNewQuota();
        }

        public static void OnNewQuota()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaInProgress = false;
            QuotaPenaltyMultiplier.Reset();
            RecordPlayersThisQuota = StartOfRound.Instance.connectedPlayersAmount;
            UpdateProfitQuota();
        }

        public static void OnPlayerCountChanged()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            var playerCount = StartOfRound.Instance.connectedPlayersAmount + 1;
            if (!StartOfRound.Instance.shipHasLanded || playerCount > RecordPlayersThisMoon)
            {
                SetRecordPlayersThisMoon(playerCount);
            }
            if (!QuotaInProgress || playerCount > RecordPlayersThisQuota)
            {
                SetRecordPlayersThisQuota(playerCount);
            }
            Plugin.Log.LogDebug("Player count: " + playerCount);
        }

        private static void LoadData()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            string saveFile = GameNetworkManager.Instance.currentSaveFileName;
            if (ES3.KeyExists(nameof(BaseProfitQuota), saveFile)) BaseProfitQuota = ES3.Load<int>(nameof(BaseProfitQuota), saveFile);
            if (ES3.KeyExists(nameof(RecordPlayersThisQuota), saveFile)) RecordPlayersThisQuota = ES3.Load<int>(nameof(RecordPlayersThisQuota), saveFile);
            if (ES3.KeyExists(nameof(QuotaInProgress), saveFile)) QuotaInProgress = ES3.Load<bool>(nameof(QuotaInProgress), saveFile);
            QuotaMultiplier.LoadAll(saveFile);
        }

        public static void SaveData()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            string saveFile = GameNetworkManager.Instance.currentSaveFileName;
            ES3.Save(nameof(BaseProfitQuota), BaseProfitQuota, saveFile);
            ES3.Save(nameof(RecordPlayersThisQuota), RecordPlayersThisQuota, saveFile);
            ES3.Save(nameof(QuotaInProgress), QuotaInProgress, saveFile);
            QuotaMultiplier.SaveAll(saveFile);
        }
    }
}
