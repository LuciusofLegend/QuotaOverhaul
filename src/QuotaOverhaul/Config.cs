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

        public static ConfigEntry<bool> creditPenaltiesEnabled;
        public static ConfigEntry<float> creditPenaltyPercentPerPlayer;
        public static ConfigEntry<bool> creditPenaltiesDynamic;
        public static ConfigEntry<float> creditPenaltyPercentCap;
        public static ConfigEntry<float> creditPenaltyPercentThreshold;
        public static ConfigEntry<float> creditPenaltyRecoveryBonus;


        public static ConfigEntry<bool> quotaPenaltiesEnabled;
        public static ConfigEntry<float> quotaPenaltyPercentPerPlayer;
        public static ConfigEntry<bool> quotaPenaltiesDynamic;
        public static ConfigEntry<float> quotaPenaltyPercentCap;
        public static ConfigEntry<float> quotaPenaltyPercentThreshold;
        public static ConfigEntry<float> quotaPenaltyRecoveryBonus;

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
            startingQuota = Plugin.config.Bind("QuotaSettings", "Starting Quota", 150, "The starting quota for the game. \nVanilla = 130 \nDefault = 300");
            quotaMinIncrease = Plugin.config.Bind("QuotaSettings", "Quota Min Increase", 100, "The minimum amount of quota increase. \nVanilla = 200 \nDefault = 200");
            quotaIncreaseSteepness = Plugin.config.Bind("QuotaSettings", "Quota Increase Steepness", 4f, "The steepness of the quota increase curve - higher value means a less steep exponential increase. \nVanilla = 4 \nDefault = 4");
            quotaRandomizerMultiplier = Plugin.config.Bind("QuotaSettings", "Quota Randomizer Multiplier", 1f, "The multiplier for the quota randomizer - this determines the severity of the randomizer curve. \nVanilla = 1 \nDefault = 1");
            quotaEnablePlayerMultiplier = Plugin.config.Bind("QuotaSettings", "Enable Player Count Multiplier", true, "Multiply the quota based on the number of players.\nVanilla = false \nDefault = true");
            quotaPlayerThreshold = Plugin.config.Bind("QuotaSettings", "Player Count Threshold", 3, "The quota multiplier will increase for every player beyond this threshold. \nDefault = 3");
            quotaPlayerCap = Plugin.config.Bind("QuotaSettings", "Player Count Cap", 8, "Adding more players beyond this cap will not increase the quota multiplier. \nDefault = 8");
            quotaMultPerPlayer = Plugin.config.Bind("QuotaSettings", "Multiplier Per Player", 0.3f, "The multiplier for each player above the threshold. \nDefault = 0.3f");

            creditPenaltiesEnabled = Plugin.config.Bind("CreditPenalties", "Credit Penalties", false, "Toggle losing credits for each player that dies.  Works just like vanilla when enabled. \nDefault = false \nVanilla = true");
            creditPenaltyPercentPerPlayer = Plugin.config.Bind("CreditPenalties", "Penalty Per Player", 20f, "The amount of credits to lose per dead player, as a percentage of current credits. \nValues >= 0 \nDefault: 20");
            creditPenaltiesDynamic = Plugin.config.Bind("CreditPenalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on what fraction of total players have died.  AKA it scales with player count. \nDefault = true");
            creditPenaltyPercentCap = Plugin.config.Bind("CreditPenalties", "Penalty Percent Cap", 80f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0 \nDefault = 50");
            creditPenaltyPercentThreshold = Plugin.config.Bind("CreditPenalties", "Penalty Threshold Percent", 20f, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving. \nValues between 0-100 \nDefault = 20");
            creditPenaltyRecoveryBonus = Plugin.config.Bind("CreditPenalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100 \nDefault = 50");

            quotaPenaltiesEnabled = Plugin.config.Bind("QuotaPenalties", "Quota Penalties", true, "Increase the quota for each player that dies. Intended to replace losing scrap when all players die. Penalties are applied as a percent of the base quota for the round. Penalties only affect the current quota, and do not carry to future rounds. Penalties are not added for deaths at the Company Building. \nDefault = true \nVanilla = false");
            quotaPenaltyPercentPerPlayer = Plugin.config.Bind("QuotaPenalties", "Penalty Per Player", 12f, "The amount to increase the quota per dead player, as a percentage of the base quota for this round. \nValues >= 0 \nDefault: 12");
            quotaPenaltiesDynamic = Plugin.config.Bind("QuotaPenalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on what fraction of total players have died.  AKA it scales with player count. \nDefault = true");
            quotaPenaltyPercentCap = Plugin.config.Bind("QuotaPenalties", "Penalty Percent Cap", 50f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0 \nDefault = 50");
            quotaPenaltyPercentThreshold = Plugin.config.Bind("QuotaPenalties", "Penalty Threshold Percent", 15f, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving. \nValues between 0-100 \nDefault = 15");
            quotaPenaltyRecoveryBonus = Plugin.config.Bind("QuotaPenalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100 \nDefault = 50");
            
            saveAllChance = Plugin.config.Bind("LootSaving", "SaveAllChance", 1f, "A chance of all items being saved. \nValues between 0-1 \nDefault = 1 \nVanilla = 0");
            saveEachChance = Plugin.config.Bind("LootSaving", "SaveEachChance", 0.5f, "A chance of each item being saved.\nApplied after SaveAllChance. \nValues between 0-1 \nDefault = 0.5 \nVanilla = 0");
            scrapLossMax = Plugin.config.Bind("LootSaving", "ScrapLossMax", int.MaxValue, $"The maximum amount of items that can be lost.\nApplied after SaveEachChance. \nDefault = {int.MaxValue}");

            valueSaveEnabled = Plugin.config.Bind("LootSaving", "ValueSaveEnabled", true, "Save a percent of total scrap value.\nApplied after SaveAllChance and prevent SaveEachChance \nDefault = true \nVanilla = false.");
            valueSavePercent = Plugin.config.Bind("LootSaving", "ValueSavePercent", 1f, "The percentage of total scrap value to save. \nValues between 0-1 \nDefault = 1");

            equipmentLossEnabled = Plugin.config.Bind("EquipmentLoss", "EquipmentLossEnabled", false, "Allow equipment to be lost. \nDefault = false \nVanilla = false.");
            equipmentLossChance = Plugin.config.Bind("EquipmentLoss", "EquipmentLossChance", 0.1f, "A chance of each equipment item being lost. \nApplied after SaveAllChance. \nValues between 0-1 \nDefault = 0.1 \nVanilla = 0");
            equipmentLossMax = Plugin.config.Bind("EquipmentLoss", "EquipmentLossMax", int.MaxValue, $"The maximum amount of equipment that can be lost.\nApplied after EquipmentLossChance. \nDefault = {int.MaxValue}");
        }
    }
}