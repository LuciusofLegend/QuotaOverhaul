using HarmonyLib;

namespace QuotaOverhaul.Patches
{
    [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))]
    public class DeathPenaltyPatch
    {
        public static bool Prefix()
        {
            return false;
        }

        public static void Postfix(int playersDead, int bodiesInsured)
        {
            DeathConsequences.ApplyDeathPenalties(playersDead, bodiesInsured);
        }
    }
}
