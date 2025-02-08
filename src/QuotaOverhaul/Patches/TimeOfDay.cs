using HarmonyLib;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Awake))]
    public class NewSessionPatch
    {
        public static void Postfix()
        {
            QuotaOverhaul.OnNewSession();
        }   
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
    public class QuotaUpdatePatch
    {
        public static bool Prefix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return false;
            if (!CanFinishQuota()) return false;
            Plugin.Log.LogInfo("Calculating New Profit Quota...");
            TimeOfDay.Instance.profitQuota = QuotaOverhaul.baseProfitQuota;
            return true;
        }

        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            if (!CanFinishQuota()) return;
            QuotaOverhaul.baseProfitQuota = TimeOfDay.Instance.profitQuota;
            QuotaOverhaul.OnNewQuota();
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