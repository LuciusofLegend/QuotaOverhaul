using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;

namespace QuotaOverhaul
{
    public class Config : SyncedConfig2<Config>
    {
        // Quota settings
        public static ConfigEntry<int> startingQuota;
        public static ConfigEntry<int> quotaBaseIncrease;
        public static ConfigEntry<float> quotaIncreaseSteepness;
        public static ConfigEntry<float> quotaRandomizerMultiplier;
        [SyncedEntryField] public static SyncedEntry<int> quotaDeadline;
        public static ConfigEntry<bool> quotaEnablePlayerMultiplier;
        public static ConfigEntry<int> quotaPlayerThreshold;
        public static ConfigEntry<int> quotaPlayerCap;
        public static ConfigEntry<float> quotaMultPerPlayer;

        [SyncedEntryField] public static SyncedEntry<bool> creditPenaltiesEnabled;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyPercentPerPlayer;
        [SyncedEntryField] public static SyncedEntry<bool> creditPenaltiesDynamic;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyPercentCap;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyPercentThreshold;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyRecoveryBonus;


        [SyncedEntryField] public static SyncedEntry<bool> quotaPenaltiesEnabled;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyPercentPerPlayer;
        [SyncedEntryField] public static SyncedEntry<bool> quotaPenaltiesDynamic;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyPercentCap;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyPercentThreshold;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyRecoveryBonus;

        public static ConfigEntry<float> saveAllChance;
        public static ConfigEntry<float> saveEachChance;
        public static ConfigEntry<int> scrapLossMax;

        public static ConfigEntry<bool> valueSaveEnabled;
        public static ConfigEntry<float> valueSavePercent;

        public static ConfigEntry<bool> equipmentLossEnabled;
        public static ConfigEntry<float> equipmentLossChance;
        public static ConfigEntry<int> equipmentLossMax;

        [SyncedEntryField] public static SyncedEntry<int> startingCredits;

        public Config(ConfigFile config) : base(Plugin.PLUGIN_GUID)
        {
            startingQuota = config.Bind("QuotaSettings", "Starting Quota", 300, "The quota value at the start of a new game. \nVanilla: 130");
            quotaBaseIncrease = config.Bind("QuotaSettings", "Quota Base Increase", 200, "The minimum amount of quota increase. \nVanilla: 200");
            quotaIncreaseSteepness = config.Bind("QuotaSettings", "Quota Increase Steepness", 4f, "The steepness of the quota increase curve - higher value means a less steep exponential increase. \nVanilla: 4");
            quotaRandomizerMultiplier = config.Bind("QuotaSettings", "Quota Randomizer Multiplier", 1f, "The multiplier for the quota randomizer - this determines the severity of the randomizer curve. \nVanilla: 1");
            quotaDeadline = config.BindSyncedEntry("QuotaSettings", "Quota Deadline", 3, "The number of days you are given to complete each quota.  \nVanilla: 3");
            quotaEnablePlayerMultiplier = config.Bind("QuotaSettings", "Enable Player Count Multiplier", true, "Multiply the quota based on the number of players.\nVanilla: false");
            quotaPlayerThreshold = config.Bind("QuotaSettings", "Player Count Threshold", 3, "The quota multiplier will increase for every player beyond this threshold.");
            quotaPlayerCap = config.Bind("QuotaSettings", "Player Count Cap", 8, "Adding more players beyond this cap will not increase the quota multiplier.");
            quotaMultPerPlayer = config.Bind("QuotaSettings", "Multiplier Per Player", 0.3f, "The multiplier for each player above the threshold.");

            creditPenaltiesEnabled = config.BindSyncedEntry("CreditPenalties", "Credit Penalties", false, "Toggle losing credits for each player that dies.  Works just like vanilla when enabled. \nVanilla: true");
            creditPenaltyPercentPerPlayer = config.BindSyncedEntry("CreditPenalties", "Penalty Per Player", 20f, "The amount of credits to lose per dead player, as a percentage of current credits. \nValues >= 0");
            creditPenaltiesDynamic = config.BindSyncedEntry("CreditPenalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on what fraction of total players have died.  AKA it scales with player count.");
            creditPenaltyPercentCap = config.BindSyncedEntry("CreditPenalties", "Penalty Percent Cap", 80f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
            creditPenaltyPercentThreshold = config.BindSyncedEntry("CreditPenalties", "Penalty Threshold Percent", 20f, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving. \nValues between 0-100");
            creditPenaltyRecoveryBonus = config.BindSyncedEntry("CreditPenalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100");

            quotaPenaltiesEnabled = config.BindSyncedEntry("QuotaPenalties", "Quota Penalties", true, "Increase the quota for each player that dies. Intended to replace losing scrap when all players die. Penalties are applied as a percent of the base quota for the round. Penalties only affect the current quota, and do not carry to future rounds. Penalties are not added for deaths at the Company Building. \nVanilla: false");
            quotaPenaltyPercentPerPlayer = config.BindSyncedEntry("QuotaPenalties", "Penalty Per Player", 12f, "The amount to increase the quota per dead player, as a percentage of the base quota for this round. \nValues >= 0");
            quotaPenaltiesDynamic = config.BindSyncedEntry("QuotaPenalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on what fraction of total players have died.  AKA it scales with player count.");
            quotaPenaltyPercentCap = config.BindSyncedEntry("QuotaPenalties", "Penalty Percent Cap", 50f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
            quotaPenaltyPercentThreshold = config.BindSyncedEntry("QuotaPenalties", "Penalty Threshold Percent", 15f, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving. \nValues between 0-100");
            quotaPenaltyRecoveryBonus = config.BindSyncedEntry("QuotaPenalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100");
            
            saveAllChance = config.Bind("LootSaving", "Save All Chance", 100f, "A percent chance of all items being saved. \nValues between 0-100 \nVanilla:0");
            saveEachChance = config.Bind("LootSaving", "Save Each Chance", 50f, "A percent chance of each item being saved.\nApplied after SaveAllChance. \nValues between 0-100 \nVanilla:0");
            scrapLossMax = config.Bind("LootSaving", "Scrap Loss Max", int.MaxValue, $"The maximum amount of items that can be lost.\nApplied after SaveEachChance.");

            valueSaveEnabled = config.Bind("LootSaving", "Value Save Enabled", true, "Save a percent of total scrap value.\nApplied after SaveAllChance and prevent SaveEachChance. \nVanilla:false.");
            valueSavePercent = config.Bind("LootSaving", "Value Save Percent", 100f, "The percentage of total scrap value to save. \nValues between 0-100");

            equipmentLossEnabled = config.Bind("EquipmentLoss", "Equipment Loss Enabled", false, "Allow equipment to be lost. \nVanilla:false.");
            equipmentLossChance = config.Bind("EquipmentLoss", "Equipment Loss Chance", 10f, "A chance of each equipment item being lost. \nApplied after SaveAllChance. \nValues between 0-100 \nVanilla: 0");
            equipmentLossMax = config.Bind("EquipmentLoss", "Equipment Loss Max", int.MaxValue, $"The maximum amount of equipment that can be lost.\nApplied after EquipmentLossChance.");

            startingCredits = config.BindSyncedEntry("OtherSettings", "Starting Credits", 60, "The amount of money you start with. \nVanilla: 60");

            ConfigManager.Register(this);
        } 
    }
}