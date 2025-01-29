using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using QuotaOverhaul;


[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("LethalNetworkAPI", "3.0.0")]
[BepInDependency("com.sigurd.csync", "5.0.0")]
public class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_GUID = LCMPluginInfo.PLUGIN_GUID;
    public const string PLUGIN_NAME = LCMPluginInfo.PLUGIN_NAME;
    public const string PLUGIN_VERSION = LCMPluginInfo.PLUGIN_VERSION; 

    public static Plugin instance;
    public static ManualLogSource Log;
    internal static new Config config;

    private void Awake()
    {
        instance = this;
        Log = BepInEx.Logging.Logger.CreateLogSource(PLUGIN_NAME);
        config = new Config(Config);
        new Harmony(PLUGIN_GUID).PatchAll();
        Log.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} is loaded!");
    }
}