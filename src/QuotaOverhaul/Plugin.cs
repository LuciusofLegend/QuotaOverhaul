using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;


[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    const string PLUGIN_GUID = LCMPluginInfo.PLUGIN_GUID, PLUGIN_NAME = LCMPluginInfo.PLUGIN_NAME, PLUGIN_VERSION = LCMPluginInfo.PLUGIN_VERSION; 

    public static Plugin instance;
    public static ManualLogSource Log;
    public static ConfigFile config;

    private void Awake()
    {
        Log = BepInEx.Logging.Logger.CreateLogSource(PLUGIN_GUID);
        instance = this;
        config = Config;
        QuotaOverhaul.Config.Load();
        new Harmony(PLUGIN_GUID).PatchAll();
        Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} is loaded!");
    }
}