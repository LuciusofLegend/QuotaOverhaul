using HarmonyLib;

namespace QuotaOverhaul
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

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SetTimeAndPlanetToSavedSettings))]
    public class LoadDataPatch
    {
        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaOverhaul.LoadData();
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        public static void Postfix()
        {
            QuotaOverhaul.OnPlayerCountChanged();
            Plugin.Log.LogInfo("Player joined");
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientDisconnect))]
    public class OnPlayerDisconnectPatch
    {
        public static void Postfix()
        {
            QuotaOverhaul.OnPlayerCountChanged();
            Plugin.Log.LogInfo("Player joined");
        }
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnShipLandedMiscEvents))]
    public class OnShipLandedPatch
    {
        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaOverhaul.quotaInProgress = true;
        }
    }
}