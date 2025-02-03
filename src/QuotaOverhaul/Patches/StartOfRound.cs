using HarmonyLib;

namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        [HarmonyPatch]
        public static void Postfix()
        {
            QuotaOverhaul.OnPlayerCountChanged();
            Plugin.Log.LogInfo("OnPlayerConnect() patched");
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientDisconnect))]
    public class OnPlayerDisconnectPatch
    {
        [HarmonyPatch]
        public static void Postfix()
        {
            QuotaOverhaul.OnPlayerCountChanged();
            Plugin.Log.LogInfo("OnPlayerDisconnect() patched");
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