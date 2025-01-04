using HarmonyLib;
using Unity.Mathematics;
namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        public void Postfix()
        {
            QuotaManager.recordPlayersThisQuota = math.max(QuotaManager.recordPlayersThisQuota, StartOfRound.Instance.connectedPlayersAmount);
            QuotaManager.recordPlayersThisMoon = math.max(QuotaManager.recordPlayersThisMoon, StartOfRound.Instance.connectedPlayersAmount);
        }
    }
}