using HarmonyLib;
using Unity.Mathematics;
namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        public static void Postfix()
        {
            QuotaOverhaul.OnPlayerConnect();
            Plugin.Log.LogInfo("OnPlayerConnect() patched");
        }
    }
}