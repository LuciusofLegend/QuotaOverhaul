using HarmonyLib;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Awake))]
    public class QuotaVariablesPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void SetQuotaVariables(ref TimeOfDay __instance)
        {
            var __quotaVariables = __instance.quotaVariables;

            __quotaVariables.startingQuota = Config.startingQuota.Value;
            __quotaVariables.baseIncrease = Config.quotaMinIncrease.Value;
            __quotaVariables.increaseSteepness = Config.quotaIncreaseSteepness.Value;
            __quotaVariables.randomizerMultiplier = Config.quotaRandomizerMultiplier.Value;
            QuotaManager.baseProfitQuota = __quotaVariables.startingQuota;

            Plugin.Log.LogInfo("Hello hello");
        }   
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
    public class QuotaUpdatePatch
    {
        [HarmonyPatch("SetNewProfitQuota")]
        public static void Prefix()
        {
            TimeOfDay.Instance.profitQuota = QuotaManager.baseProfitQuota;
            Plugin.Log.LogInfo($"Profit Quota: {TimeOfDay.Instance.profitQuota}");
            Plugin.Log.LogInfo("Calculating New Profit Quota...");
        }

        [HarmonyPatch("SetNewProfitQuota")]
        public static void Postfix()
        {
            QuotaManager.baseProfitQuota = TimeOfDay.Instance.profitQuota;
            Plugin.Log.LogInfo($"Profit Quota: {QuotaManager.baseProfitQuota}");
        }
    }
}