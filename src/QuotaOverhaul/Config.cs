using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;

namespace QuotaOverhaul
{
    public class Config : SyncedConfig2<Config>
    {
        // Quota settings
        [SyncedEntryField] public static SyncedEntry<int> startingCredits;
        [SyncedEntryField] public static SyncedEntry<int> startingQuota;
        [SyncedEntryField] public static SyncedEntry<int> quotaBaseIncrease;
        [SyncedEntryField] public static SyncedEntry<float> quotaIncreaseSteepness;
        [SyncedEntryField] public static SyncedEntry<float> quotaRandomizerMultiplier;
        [SyncedEntryField] public static SyncedEntry<int> quotaDeadline;
        [SyncedEntryField] public static SyncedEntry<int> quotaEarlyFinishLine;
        [SyncedEntryField] public static SyncedEntry<bool> quotaEnablePlayerMultiplier;
        [SyncedEntryField] public static SyncedEntry<int> quotaPlayerThreshold;
        [SyncedEntryField] public static SyncedEntry<int> quotaPlayerCap;
        [SyncedEntryField] public static SyncedEntry<float> quotaMultPerPlayer;

        [SyncedEntryField] public static SyncedEntry<bool> creditPenaltiesEnabled;
        [SyncedEntryField] public static SyncedEntry<bool> creditPenaltiesOnGordion;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyPercentPerPlayer;
        [SyncedEntryField] public static SyncedEntry<bool> creditPenaltiesDynamic;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyPercentCap;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyPercentThreshold;
        [SyncedEntryField] public static SyncedEntry<float> creditPenaltyRecoveryBonus;


        [SyncedEntryField] public static SyncedEntry<bool> quotaPenaltiesEnabled;
        [SyncedEntryField] public static SyncedEntry<bool> quotaPenaltiesOnGordion;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyPercentPerPlayer;
        [SyncedEntryField] public static SyncedEntry<bool> quotaPenaltiesDynamic;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyPercentCap;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyPercentThreshold;
        [SyncedEntryField] public static SyncedEntry<float> quotaPenaltyRecoveryBonus;

        [SyncedEntryField] public static SyncedEntry<bool> scrapLossEnabled;
        [SyncedEntryField] public static SyncedEntry<float> itemsSafeChance;
        [SyncedEntryField] public static SyncedEntry<float> loseEachScrapChance;
        [SyncedEntryField] public static SyncedEntry<int> maxLostScrapItems;

        [SyncedEntryField] public static SyncedEntry<bool> valueLossEnabled;
        [SyncedEntryField] public static SyncedEntry<float> valueLossPercent;

        [SyncedEntryField] public static SyncedEntry<bool> equipmentLossEnabled;
        [SyncedEntryField] public static SyncedEntry<float> loseEachEquipmentChance;
        [SyncedEntryField] public static SyncedEntry<int> maxLostEquipmentItems;

        public Config(ConfigFile config) : base(Plugin.PLUGIN_GUID)
        {
            startingCredits = config.BindSyncedEntry("Quota Settings", "Starting Credits", 60, "The amount of money you start with. \nVanilla: 60");
            startingQuota = config.BindSyncedEntry("Quota Settings", "Starting Quota", 300, "The quota value at the start of a new game. \nVanilla: 130");
            quotaBaseIncrease = config.BindSyncedEntry("Quota Settings", "Quota Base Increase", 200, "The minimum amount of quota increase. \nVanilla: 200");
            quotaIncreaseSteepness = config.BindSyncedEntry("Quota Settings", "Quota Increase Steepness", 4f, "The steepness of the quota increase curve - higher value means a more steep exponential increase. \nVanilla: 4");
            quotaRandomizerMultiplier = config.BindSyncedEntry("Quota Settings", "Quota Randomizer Multiplier", 1f, "The multiplier for the quota randomizer - this determines the severity of the randomizer curve. \nVanilla: 1");
            quotaDeadline = config.BindSyncedEntry("Quota Settings", "Quota Deadline", 3, "The number of days you are given to complete each quota.  \nVanilla: 3");
            quotaEarlyFinishLine = config.BindSyncedEntry("Quota Settings", "Early Finish Line", 0, "The minimum number of days that need to pass before the quota is allowed to end.  Values lower than 0 make this equal to the Quota Deadline. \nVanilla: 0");
            quotaEnablePlayerMultiplier = config.BindSyncedEntry("Quota Settings", "Enable Player Count Multiplier", true, "Multiply the quota based on the number of players.\nVanilla: false");
            quotaPlayerThreshold = config.BindSyncedEntry("Quota Settings", "Player Count Threshold", 3, "The quota multiplier will increase for every player beyond this threshold.");
            quotaPlayerCap = config.BindSyncedEntry("Quota Settings", "Player Count Cap", 8, "Adding more players beyond this cap will not increase the quota multiplier.");
            quotaMultPerPlayer = config.BindSyncedEntry("Quota Settings", "Multiplier Per Player", 0.3f, "The multiplier for each player above the threshold.");

            creditPenaltiesEnabled = config.BindSyncedEntry("Credit Penalties", "Enable", false, "Toggle losing credits for each player that dies.  Works just like vanilla when enabled. \nVanilla: true");
            creditPenaltiesOnGordion = config.BindSyncedEntry("Credit Penalties", "Enable At The Company", false, "Whether to allow credit penalties at the company building.");
            creditPenaltyPercentPerPlayer = config.BindSyncedEntry("Credit Penalties", "Penalty Per Player", 20f, "The amount of credits to lose per dead player, as a percentage of current credits. \nValues >= 0");
            creditPenaltiesDynamic = config.BindSyncedEntry("Credit Penalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on what fraction of total players have died.  AKA it scales with player count.");
            creditPenaltyPercentCap = config.BindSyncedEntry("Credit Penalties", "Penalty Percent Cap", 80f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
            creditPenaltyPercentThreshold = config.BindSyncedEntry("Credit Penalties", "Penalty Threshold Percent", 20f, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving. \nValues between 0-100");
            creditPenaltyRecoveryBonus = config.BindSyncedEntry("Credit Penalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100");

            quotaPenaltiesEnabled = config.BindSyncedEntry("Quota Penalties", "Enable", true, "Increase the quota for each player that dies. Intended to replace losing scrap when all players die. Penalties are applied as a percent of the base quota for the round. Penalties only affect the current quota, and do not carry to future rounds. \nVanilla: false");
            quotaPenaltiesOnGordion = config.BindSyncedEntry("Quota Penalties", "Enable At The Company", false, "Whether to allow quota penalties at the company building. \nVanilla: true");
            quotaPenaltyPercentPerPlayer = config.BindSyncedEntry("Quota Penalties", "Penalty Per Player", 12f, "The amount to increase the quota per dead player, as a percentage of the base quota for this round. \nValues >= 0");
            quotaPenaltiesDynamic = config.BindSyncedEntry("Quota Penalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on what fraction of total players have died.  AKA it scales with player count.");
            quotaPenaltyPercentCap = config.BindSyncedEntry("Quota Penalties", "Penalty Percent Cap", 50f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
            quotaPenaltyPercentThreshold = config.BindSyncedEntry("Quota Penalties", "Penalty Threshold Percent", 15f, "Applied after penalty is calculated. If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving. \nValues between 0-100");
            quotaPenaltyRecoveryBonus = config.BindSyncedEntry("Quota Penalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100");
            
            scrapLossEnabled = config.BindSyncedEntry("Scrap Loss", "Scrap Loss Enabled", false, "Toggle losing scrap when all players die. \nVanilla: true");
            itemsSafeChance = config.BindSyncedEntry("Scrap Loss", "Safe Chance", 25f, "A percent chance of all scrap and equipment being safe. When your items are 'safe' it overrides all other settings. \nValues between 0-100 \nVanilla: 0");
            valueLossEnabled = config.BindSyncedEntry("Scrap Loss", "Value Loss Enabled", false, "Lose a percent of total scrap value. \nVanilla: false.");
            valueLossPercent = config.BindSyncedEntry("Scrap Loss", "Value Loss Percent", 100f, "The percentage of total scrap value to lose. \nValues between 0-100");
            loseEachScrapChance = config.BindSyncedEntry("Scrap Loss", "Lose Each Chance", 50f, "A percent chance of each item being lost. \nValues between 0-100 \nVanilla: 0");
            maxLostScrapItems = config.BindSyncedEntry("Scrap Loss", "Scrap Loss Max", int.MaxValue, $"The maximum number of scrap items that can be lost.");

            equipmentLossEnabled = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Enabled", false, "Allow equipment to be lost. \nVanilla: false.");
            loseEachEquipmentChance = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Chance", 10f, "A chance of each equipment item being lost. \nApplied after SaveAllChance. \nValues between 0-100 \nVanilla: 0");
            maxLostEquipmentItems = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Max", int.MaxValue, $"The maximum amount of equipment that can be lost.\nApplied after EquipmentLossChance.");

            ConfigManager.Register(this);
        }
    }
}