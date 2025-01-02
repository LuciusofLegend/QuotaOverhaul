using System;
using HarmonyLib;
using Unity.Collections;
namespace QuotaOverhaul
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientConnect))]
    public class OnPlayerConnectPatch
    {
        public void Postfix()
        {
            float multiplier = 1 + Math.Max(StartOfRound.Instance.connectedPlayersAmount - Config.quotaPlayerThreshold.Value, 0) * Config.quotaMultPerPlayer.Value;
            QuotaManager.quotaMultiplier = Math.Max(multiplier, QuotaManager.quotaMultiplier);
        }
    }
}