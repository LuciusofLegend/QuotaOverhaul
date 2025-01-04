using HarmonyLib;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay))]
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
            QuotaManager.baseProfitQuota = quotaVariables.startingQuota;
        }
    }

    [HarmonyPatch(typeof(TimeOfDay))]
    public class QuotaUpdatePatch
    {
        [HarmonyPatch("SetNewProfitQuota")]
        public static void Prefix()
        {
            TimeOfDay.Instance.profitQuota = QuotaManager.baseProfitQuota;
        }

        [HarmonyPatch("SetNewProfitQuota")]
        public static void Postfix()
        {
            QuotaManager.baseProfitQuota = TimeOfDay.Instance.profitQuota;
        }
    }
}