using BepInEx.Configuration;

namespace QuotaOverhaul
{
    public class Config
    {
        // Quota settings
        public static ConfigEntry<int> startingQuota;
        public static ConfigEntry<int> quotaMinIncrease;
        public static ConfigEntry<float> quotaIncreaseSteepness;
        public static ConfigEntry<float> quotaRandomizerMultiplier;
        public static ConfigEntry<bool> quotaEnablePlayerMultiplier;
        public static ConfigEntry<int> quotaPlayerThreshold;
        public static ConfigEntry<float> quotaMultPerPlayer;

        public static ConfigEntry<float> saveAllChance;
        public static ConfigEntry<float> saveEachChance;
        public static ConfigEntry<int> scrapLossMax;

        public static ConfigEntry<bool> valueSaveEnabled;
        public static ConfigEntry<float> valueSavePercent;

        public static ConfigEntry<bool> equipmentLossEnabled;
        public static ConfigEntry<float> equipmentLossChance;
        public static ConfigEntry<int> equipmentLossMax;

        public static void Load()
        {
            startingQuota = Plugin.config.Bind("QuotaSettings", "Starting Quota", 300, "The starting quota for the game. \nVanilla = 130 \nDefault = 300");
            quotaMinIncrease = Plugin.config.Bind("QuotaSettings", "Quota Min Increase", 300, "The minimum amount of quota increase.");
            quotaIncreaseSteepness = Plugin.config.Bind("QuotaSettings", "Quota Increase Steepness", 4f, "The steepness of the quota increase curve - higher value means a less steep exponential increase.");
            quotaRandomizerMultiplier = Plugin.config.Bind("QuotaSettings", "Quota Randomizer Multiplier", 1f, "The multiplier for the quota randomizer - this determines the severity of the randomizer curve.");
            quotaEnablePlayerMultiplier = Plugin.config.Bind("QuotaSettings", "Enable Player Count Multiplier", true, "Toggles multiplying the quota based on the number of players.  . \nVanilla = false \nDefault = true");
            quotaPlayerThreshold = Plugin.config.Bind("QuotaSettings", "Player Count Threshold", 4, "If the player count is equal to or less than this threshold, the quota will be not be multiplied.  For each player beyond this threshold, the quota will be multiplied by multPerPlayer.");
            quotaMultPerPlayer = Plugin.config.Bind("QuotaSettings", "Multiplier Per Player", 0.5f, "Quota will be multiplied by 1 + multPerPlayer * (playerCount - playerCountThreshold)");

            saveAllChance = Plugin.config.Bind<float>("LootSaving", "SaveAllChance", 0.25f, "A chance of all item being saved.\nVanilla value 0. Values between 0-1.");
            saveEachChance = Plugin.config.Bind<float>("LootSaving", "SaveEachChance", 0.5f, "A chance of each item being saved.\nApplied after SaveAllChance\nVanilla value 0. Values between 0-1.");
            scrapLossMax = Plugin.config.Bind<int>("LootSaving", "ScrapLossMax", int.MaxValue, $"The maximum amount of items that can be lost.\nApplied after SaveEachChance\nVanilla value {int.MaxValue}. Values between 0-{int.MaxValue}.");

            valueSaveEnabled = Plugin.config.Bind<bool>("LootSaving", "ValueSaveEnabled", false, "Will it try to save item based on it scrap value?\nApplied after SaveAllChance and prevent SaveEachChance\nVanilla value False.");
            valueSavePercent = Plugin.config.Bind<float>("LootSaving", "ValueSavePercent", 0.25f, "What percentage of total scrap value will be saved among loot.\nVanilla value 0. Values between 0-1.");

            equipmentLossEnabled = Plugin.config.Bind<bool>("EquipmentLoss", "EquipmentLossEnabled", false, "Will it allow equipment to be lost?\nVanilla value False.");
            equipmentLossChance = Plugin.config.Bind<float>("EquipmentLoss", "EquipmentLossChance", 0.1f, "A chance of each equipment being lost.\nApplied after SaveAllChance\nVanilla value 0. Values between 0-1.");
            equipmentLossMax = Plugin.config.Bind<int>("EquipmentLoss", "EquipmentLossMax", int.MaxValue, $"The maximum amount of equipment that can be lost.\nApplied after EquipmentLossChance\nVanilla value 0. Values between 0-{int.MaxValue}.");
        }
    }
}