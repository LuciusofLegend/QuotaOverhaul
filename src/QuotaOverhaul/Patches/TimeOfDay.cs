using HarmonyLib;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Awake))]
    public class QuotaVariablesPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void SetQuotaVariables()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            var quotaVariables = TimeOfDay.Instance.quotaVariables;

            quotaVariables.startingQuota = Config.startingQuota.Value;
            QuotaOverhaul.baseProfitQuota = quotaVariables.startingQuota;
            quotaVariables.baseIncrease = Config.quotaBaseIncrease.Value;
            quotaVariables.increaseSteepness = Config.quotaIncreaseSteepness.Value;
            quotaVariables.randomizerMultiplier = Config.quotaRandomizerMultiplier.Value;
            quotaVariables.startingCredits = Config.startingCredits;
            quotaVariables.deadlineDaysAmount = Config.quotaDeadline;

            TimeOfDay.Instance.quotaVariables = quotaVariables;

            QuotaOverhaul.UpdateProfitQuota();

            Plugin.Log.LogInfo("Quota Variables Configured");
        }   
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
    public class QuotaUpdatePatch
    {
        [HarmonyPatch("SetNewProfitQuota")]
        public static bool Prefix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return false;
            if (!CanFinishQuota()) return false;
            Plugin.Log.LogInfo("Calculating New Profit Quota...");
            return true;
        }

        [HarmonyPatch("SetNewProfitQuota")]
        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            if (!CanFinishQuota()) return;
            QuotaOverhaul.quotaInProgress = false;
            QuotaOverhaul.quotaPenaltyMultiplier = 1;
            QuotaOverhaul.recordPlayersThisQuota = StartOfRound.Instance.connectedPlayersAmount;
            QuotaOverhaul.baseProfitQuota = TimeOfDay.Instance.profitQuota;
            QuotaOverhaul.UpdateProfitQuota();
        }

        public static bool CanFinishQuota()
        {
            int daysSinceQuotaStart = TimeOfDay.Instance.quotaVariables.deadlineDaysAmount - TimeOfDay.Instance.daysUntilDeadline;
            int quotaEarlyFinishLine = Config.quotaEarlyFinishLine;
            if (quotaEarlyFinishLine < 0) quotaEarlyFinishLine = TimeOfDay.Instance.quotaVariables.deadlineDaysAmount;
            if (daysSinceQuotaStart < quotaEarlyFinishLine) return false;
            else return true;
        }
    }
}