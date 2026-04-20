using HarmonyLib;

namespace QuotaOverhaul.Patches
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
            TimeOfDay.Instance.profitQuota = QuotaOverhaul.GetBaseProfitQuota();
            Plugin.Log.LogInfo($"Calculating new Base Quota, based on a quota of {TimeOfDay.Instance.profitQuota}");
            Plugin.Log.LogInfo($"Base Increase: {TimeOfDay.Instance.quotaVariables.baseIncrease}");
            Plugin.Log.LogInfo($"Increase Steepness: {TimeOfDay.Instance.quotaVariables.increaseSteepness}");
            Plugin.Log.LogInfo($"Randomness Mult: {TimeOfDay.Instance.quotaVariables.randomizerMultiplier}");
            return true;
        }

        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            if (!CanFinishQuota()) return;
            Plugin.Log.LogInfo($"New Base Quota is {TimeOfDay.Instance.profitQuota}");
            QuotaOverhaul.SetBaseProfitQuota(TimeOfDay.Instance.profitQuota);
            QuotaOverhaul.OnNewQuota();
        }

        private static bool CanFinishQuota()
        {
            int daysSinceQuotaStart = TimeOfDay.Instance.quotaVariables.deadlineDaysAmount - TimeOfDay.Instance.daysUntilDeadline;
            if (Plugin.Config.QuotaEarlyFinishLine.Value < 0 && TimeOfDay.Instance.daysUntilDeadline > 0) return false;
            if (daysSinceQuotaStart < Plugin.Config.QuotaEarlyFinishLine)
            {
                Plugin.Log.LogInfo($"Could not finish quota.  We are {daysSinceQuotaStart} days into the quota, and we can't finish until day {Plugin.Config.QuotaEarlyFinishLine.Value}");
                return false;
            }
            else
            {
                Plugin.Log.LogInfo($"We are able to finish the quota.  We are {daysSinceQuotaStart} days into the quota, and we can finish any time after day {Plugin.Config.QuotaEarlyFinishLine.Value}");
                return true;
            }
        }
    }
}
