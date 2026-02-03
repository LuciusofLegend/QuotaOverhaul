using LethalNetworkAPI;
using Unity.Mathematics;

namespace QuotaOverhaul
{
    public static class QuotaOverhaul
    {
        public static readonly LNetworkVariable<int> SyncedProfitQuota = LNetworkVariable<int>.Connect(nameof(SyncedProfitQuota), onValueChanged: SyncProfitQuota);

        public static void SyncProfitQuota(int oldValue, int newValue)
        {
            TimeOfDay.Instance.profitQuota = newValue;
        }

        private static int BaseProfitQuota;
        private static double QuotaPenaltyMultiplier = 1;
        private static double QuotaPlayerMultiplier = 1;
        private static int RecordPlayersThisQuota = 1;
        private static int RecordPlayersThisMoon = 1;
        public static bool QuotaInProgress;

        public static int GetBaseProfitQuota() {
            return BaseProfitQuota;
        }

        public static void SetBaseProfitQuota(int value) {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            BaseProfitQuota = value;
            SyncedProfitQuota.Value = CalculateProfitQuota();
        }

        public static double GetQuotaPenaltyMultiplier() {
            return QuotaPenaltyMultiplier;
        }

        public static void SetQuotaPenaltyMultiplier(double value) {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaPenaltyMultiplier = value;
            SyncedProfitQuota.Value = CalculateProfitQuota();
        }

        public static void AddQuotaPenaltyMultiplier(double value) {
            SetQuotaPenaltyMultiplier(GetQuotaPenaltyMultiplier() + value);
        }

        public static double GetQuotaPlayerMultiplier() {
            return QuotaPlayerMultiplier;
        }

        public static void SetQuotaPlayerMultiplier(double value) {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaPlayerMultiplier = value;
            SyncedProfitQuota.Value = CalculateProfitQuota();
        }

        public static int GetRecordPlayersThisQuota() {
            return RecordPlayersThisQuota;
        }

        public static void SetRecordPlayersThisQuota(int value) {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            RecordPlayersThisQuota = value;
            QuotaPlayerMultiplier = CalculatePlayerCountMultiplier();
            SyncedProfitQuota.Value = CalculateProfitQuota();
        }

        public static int GetRecordPlayersThisMoon() {
            return RecordPlayersThisMoon;
        }

        public static void SetRecordPlayersThisMoon(int value) {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            RecordPlayersThisQuota = value;
        }

        public static int CalculateProfitQuota(bool usePlayerCountMultiplier = true, bool usePenaltyMultiplier = true)
        {
            int result = BaseProfitQuota;
            if (usePlayerCountMultiplier) result = (int)(result * QuotaPlayerMultiplier);
            if (usePenaltyMultiplier) result = (int)(result * QuotaPenaltyMultiplier);
            return result;
        }

        public static double CalculatePlayerCountMultiplier()
        {
            int playersCounted = math.clamp(RecordPlayersThisQuota, Config.QuotaPlayerThreshold.Value, Config.QuotaPlayerCap.Value);
            playersCounted -= Config.QuotaPlayerThreshold.Value;
            return 1 + Config.QuotaMultiplierPerPlayer.Value * math.max(playersCounted, 0);
        }

        public static void OnNewSession()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            if (Config.StartingQuota != 130) TimeOfDay.Instance.quotaVariables.startingQuota = Config.StartingQuota.Value;
            if (Config.QuotaBaseIncrease != 200) TimeOfDay.Instance.quotaVariables.baseIncrease = Config.QuotaBaseIncrease.Value;
            if (Config.QuotaIncreaseSteepness != 4f) TimeOfDay.Instance.quotaVariables.increaseSteepness = Config.QuotaIncreaseSteepness.Value;
            if (Config.QuotaRandomizerMultiplier != 1f) TimeOfDay.Instance.quotaVariables.randomizerMultiplier = Config.QuotaRandomizerMultiplier.Value;
            if (Config.StartingCredits != 60) TimeOfDay.Instance.quotaVariables.startingCredits = Config.StartingCredits.Value;
            if (Config.QuotaDeadline != 3) TimeOfDay.Instance.quotaVariables.deadlineDaysAmount = Config.QuotaDeadline.Value;
            LoadData();
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
            QuotaPenaltyMultiplier = 1;
            RecordPlayersThisQuota = StartOfRound.Instance.connectedPlayersAmount;
            SyncedProfitQuota.Value = CalculateProfitQuota();
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
        }

        private static void LoadData()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            string saveFile = GameNetworkManager.Instance.currentSaveFileName;
            if (ES3.KeyExists(nameof(BaseProfitQuota), saveFile)) BaseProfitQuota = ES3.Load<int>(nameof(BaseProfitQuota), saveFile);
            if (ES3.KeyExists(nameof(QuotaPenaltyMultiplier), saveFile)) QuotaPenaltyMultiplier = ES3.Load<double>(nameof(QuotaPenaltyMultiplier), saveFile);
            if (ES3.KeyExists(nameof(RecordPlayersThisQuota), saveFile)) RecordPlayersThisQuota = ES3.Load<int>(nameof(RecordPlayersThisQuota), saveFile);
            if (ES3.KeyExists(nameof(QuotaInProgress), saveFile)) QuotaInProgress = ES3.Load<bool>(nameof(QuotaInProgress), saveFile);
            if (TimeOfDay.Instance.timesFulfilledQuota == 0 && !QuotaInProgress) OnNewRun();
        }

        public static void SaveData()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;

            string saveFile = GameNetworkManager.Instance.currentSaveFileName;
            ES3.Save(nameof(BaseProfitQuota), BaseProfitQuota, saveFile);
            ES3.Save(nameof(QuotaPenaltyMultiplier), QuotaPenaltyMultiplier, saveFile);
            ES3.Save(nameof(RecordPlayersThisQuota), RecordPlayersThisQuota, saveFile);
            ES3.Save(nameof(QuotaInProgress), QuotaInProgress, saveFile);
        }
    }
}
