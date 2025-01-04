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
        public static ConfigEntry<int> quotaPlayerCap;
        public static ConfigEntry<float> quotaMultPerPlayer;

        public static ConfigEntry<float> saveAllChance;
        public static ConfigEntry<float> saveEachChance;
        public static ConfigEntry<int> scrapLossMax;

        public static ConfigEntry<bool> valueSaveEnabled;
        public static ConfigEntry<float> valueSavePercent;

        public static ConfigEntry<bool> equipmentLossEnabled;
        public static ConfigEntry<float> equipmentLossChance;
        public static ConfigEntry<int> equipmentLossMax;

        public static ConfigEntry<bool> quotaPenaltiesEnabled;
        public static ConfigEntry<int> penaltyMaxPercent;
        public static ConfigEntry<int> bodyRecoveryBonus;
        public static ConfigEntry<int> penaltyPercentThreshold;

        public static void Load()
        {
            startingQuota = Plugin.config.Bind("QuotaSettings", "Starting Quota", 150, "The starting quota for the game. \nVanilla = 130 \nDefault = 300");
            quotaMinIncrease = Plugin.config.Bind("QuotaSettings", "Quota Min Increase", 100, "The minimum amount of quota increase. \nVanilla = 200 \nDefault = 200");
            quotaIncreaseSteepness = Plugin.config.Bind("QuotaSettings", "Quota Increase Steepness", 4f, "The steepness of the quota increase curve - higher value means a less steep exponential increase. \nVanilla = 4 \nDefault = 4");
            quotaRandomizerMultiplier = Plugin.config.Bind("QuotaSettings", "Quota Randomizer Multiplier", 1f, "The multiplier for the quota randomizer - this determines the severity of the randomizer curve. \nVanilla = 1 \nDefault = 1");
            quotaEnablePlayerMultiplier = Plugin.config.Bind("QuotaSettings", "Enable Player Count Multiplier", true, "Multiply the quota based on the number of players.\nVanilla = false \nDefault = true");
            quotaPlayerThreshold = Plugin.config.Bind("QuotaSettings", "Player Count Threshold", 3, "If playerCount exceeds the threshold, the quota will be increased accordingly. \nDefault = 3");
            quotaPlayerCap = Plugin.config.Bind("QuotaSettings", "Player Count Cap", 8, "If the player count exceeds the cap, the quota will be multiplied as if the player count was equal to the cap. \nDefault = 8");
            quotaMultPerPlayer = Plugin.config.Bind("QuotaSettings", "Multiplier Per Player", 0.3f, "Quota will be multiplied by 1 + multiplierPerPlayer * (playerCount - playerCountThreshold). \nDefault = 0.3f");

            saveAllChance = Plugin.config.Bind<float>("LootSaving", "SaveAllChance", 0.25f, "A chance of all items being saved. \nValues between 0-1 \nDefault = 0.25 \nVanilla = 0");
            saveEachChance = Plugin.config.Bind<float>("LootSaving", "SaveEachChance", 0.5f, "A chance of each item being saved.\nApplied after SaveAllChance. \nValues between 0-1 \nDefault = 0.5 \nVanilla = 0");
            scrapLossMax = Plugin.config.Bind<int>("LootSaving", "ScrapLossMax", int.MaxValue, $"The maximum amount of items that can be lost.\nApplied after SaveEachChance. \nDefault = {int.MaxValue}");

            valueSaveEnabled = Plugin.config.Bind<bool>("LootSaving", "ValueSaveEnabled", true, "Save a percent of total scrap value.\nApplied after SaveAllChance and prevent SaveEachChance \nDefault = true \nVanilla = false.");
            valueSavePercent = Plugin.config.Bind<float>("LootSaving", "ValueSavePercent", 1f, "The percentage of total scrap value to save. \nValues between 0-1 \nDefault = 1");

            equipmentLossEnabled = Plugin.config.Bind<bool>("EquipmentLoss", "EquipmentLossEnabled", false, "Allow equipment to be lost. \nDefault = false \nVanilla = false.");
            equipmentLossChance = Plugin.config.Bind<float>("EquipmentLoss", "EquipmentLossChance", 0.1f, "A chance of each equipment item being lost. \nApplied after SaveAllChance. \nValues between 0-1 \nDefault = 0.1 \nVanilla = 0");
            equipmentLossMax = Plugin.config.Bind<int>("EquipmentLoss", "EquipmentLossMax", int.MaxValue, $"The maximum amount of equipment that can be lost.\nApplied after EquipmentLossChance. \nDefault = {int.MaxValue}");

            quotaPenaltiesEnabled = Plugin.config.Bind<bool>("QuotaPenalties", "Enable Death Penalties", true, "Increase the quota for each player that dies. Intended to replace losing scrap when all players die. Penalties are applied as a percent of the base quota for the round. Penalties only affect the current quota, and do not carry to future rounds. Penalties are not added for deaths at the Company Building. \nDefault = true \nVanilla = false");
            penaltyMaxPercent = Plugin.config.Bind<int>("QuotaPenalties", "Penalty Max Percent", 50, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty.");
            bodyRecoveryBonus = Plugin.config.Bind<int>("QuotaPenalties", "Body Recovery Bonus", 50, "How much of the penalty to forgive for recovering bodies. A higher value means a bigger difference. \nValues between 0-100 \nDefault = 50");
            penaltyPercentThreshold = Plugin.config.Bind<int>("QuotaPenalties", "Penalty Threshold Percent", 0, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. \nDefault = 0");
        }
    }
}