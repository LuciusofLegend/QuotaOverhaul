using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace QuotaOverhaul;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("LethalNetworkAPI", "3.3.0")]
[BepInDependency("com.sigurd.csync", "5.0.0")]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = LCMPluginInfo.PLUGIN_GUID;
    public const string PluginName = LCMPluginInfo.PLUGIN_NAME;
    public const string PluginVersion = LCMPluginInfo.PLUGIN_VERSION; 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static Plugin Instance;
    public static Harmony Harmony;
    public static ManualLogSource Log;
    internal static Config config;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private void Awake()
    {
        Instance = this;
        Harmony = new Harmony(PluginGuid);
        Log = BepInEx.Logging.Logger.CreateLogSource(PluginName);
        config = new Config(Config);
        Harmony.PatchAll();
        Log.LogInfo($"{PluginName} v{PluginVersion} is loaded!");
    }
}