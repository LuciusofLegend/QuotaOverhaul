using BepInEx;
using System.Runtime.CompilerServices;
using System;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;


[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string MOD_GUID = "LuciusofLegend.QuotaOverhaul";
    private const string MOD_NAME = "Quota Overhaul";

    public static Plugin instance;
    public static Harmony harmony = new Harmony("LethalCompany.LuciusofLegend.QuotaOverhaul");

    public static ManualLogSource Log;
    public static ConfigFile config;

    private void Awake()
    {
        Log = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);
        config = Config;
        QuotaOverhaul.Config.Load();
        instance = this;
        try
        {
            RuntimeHelpers.RunClassConstructor(typeof(QuotaOverhaul.SaveLootPatch).TypeHandle);
        }
        catch (Exception ex)
        {
            Log.LogError(string.Concat("Error in static constructor of ", typeof(QuotaOverhaul.SaveLootPatch), ": ", ex));
        }
        Log.LogInfo($"{MOD_NAME} is loaded!");
    }

}
