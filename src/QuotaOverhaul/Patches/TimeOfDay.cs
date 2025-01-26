using HarmonyLib;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Awake))]
    public class QuotaVariablesPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void SetQuotaVariables()
        {
            var quotaVariables = TimeOfDay.Instance.quotaVariables;

            quotaVariables.startingQuota = Config.startingQuota.Value;
            quotaVariables.baseIncrease = Config.quotaMinIncrease.Value;
            quotaVariables.increaseSteepness = Config.quotaIncreaseSteepness.Value;
            quotaVariables.randomizerMultiplier = Config.quotaRandomizerMultiplier.Value;
            QuotaOverhaul.baseProfitQuota = quotaVariables.startingQuota;

            Plugin.Log.LogInfo("Quota Variables Configured");
        }   
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
    public class QuotaUpdatePatch
    {
        [HarmonyPatch("SetNewProfitQuota")]
        public static void Prefix()
        {
            TimeOfDay.Instance.profitQuota = QuotaOverhaul.baseProfitQuota;
            Plugin.Log.LogInfo($"Profit Quota: {TimeOfDay.Instance.profitQuota}");
            Plugin.Log.LogInfo("Calculating New Profit Quota...");
        }

        [HarmonyPatch("SetNewProfitQuota")]
        public static void Postfix()
        {
            QuotaOverhaul.baseProfitQuota = TimeOfDay.Instance.profitQuota;
            Plugin.Log.LogInfo($"Profit Quota: {QuotaOverhaul.baseProfitQuota}");
        }
    }
}