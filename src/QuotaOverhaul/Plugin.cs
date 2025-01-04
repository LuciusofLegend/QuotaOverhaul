using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;


[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string MOD_GUID = "LuciusofLegend.QuotaOverhaul";
    private const string MOD_NAME = "Quota Overhaul";

    public static Plugin instance;
    public static Harmony harmony;

    public static ManualLogSource Log;
    public static ConfigFile config;

    private void Awake()
    {
        Log = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);
        instance = this;
        config = Config;
        harmony = new Harmony($"LethalCompany.{MOD_GUID}");
        QuotaOverhaul.Config.Load();
        harmony.PatchAll();
        Log.LogInfo($"{MOD_NAME} is loaded!");
    }
}
