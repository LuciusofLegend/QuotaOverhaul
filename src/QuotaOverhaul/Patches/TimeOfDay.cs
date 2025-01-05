using HarmonyLib;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Awake))]
    public class QuotaVariablesPatch
    {
        [HarmonyPatch("Awake")]
        public static void Prefix()
        {
            var quotaVariables = TimeOfDay.Instance.quotaVariables;

            quotaVariables.startingQuota = Config.startingQuota.Value;
            quotaVariables.baseIncrease = Config.quotaMinIncrease.Value;
            quotaVariables.increaseSteepness = Config.quotaIncreaseSteepness.Value;
            quotaVariables.randomizerMultiplier = Config.quotaRandomizerMultiplier.Value;
            QuotaManager.baseProfitQuota = quotaVariables.startingQuota;
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SetNewProfitQuota))]
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