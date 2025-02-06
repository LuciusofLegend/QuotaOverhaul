using HarmonyLib;

namespace QuotaOverhaul{
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveGame))]
    public class SaveGamePatch
    {
        [HarmonyPatch]
        public static void Postfix()
        {
            QuotaOverhaul.SaveData();
        }
    }
}