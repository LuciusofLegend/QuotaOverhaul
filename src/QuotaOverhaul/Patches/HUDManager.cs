using HarmonyLib;

namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))]
    public class DeathPenaltyPatch
    {
        
    }
}