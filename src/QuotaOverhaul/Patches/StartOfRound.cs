using HarmonyLib;
namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        [HarmonyPatch]
        public static void Postfix()
        {
            QuotaOverhaul.OnPlayerConnect();
            Plugin.Log.LogInfo("OnPlayerConnect() patched");
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnShipLandedMiscEvents))]
    public class OnShipLandedPatch
    {
        [HarmonyPatch]
        public static void Postfix()
        {
            QuotaOverhaul.quotaInProgress = true;
        }
    }
}