using HarmonyLib;

namespace QuotaOverhaul.Patches
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ResetShip))]
    public class ResetShipPatch
    {
        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaOverhaul.OnNewRun();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        public static void Postfix()
        {
            Plugin.Log.LogInfo("Player joined");
            QuotaOverhaul.OnPlayerCountChanged();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientDisconnect))]
    public class OnPlayerDisconnectPatch
    {
        public static void Postfix()
        {
            Plugin.Log.LogInfo("Player disconnected");
            QuotaOverhaul.OnPlayerCountChanged();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnShipLandedMiscEvents))]
    public class OnShipLandedPatch
    {
        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaOverhaul.QuotaInProgress = true;
        }
    }
}