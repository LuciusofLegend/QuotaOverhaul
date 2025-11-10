using HarmonyLib;

namespace QuotaOverhaul.Patches{
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveGame))]
    public class SaveGamePatch
    {
        public static void Postfix()
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            QuotaOverhaul.SaveData();
        }
    }
}