using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;

namespace QuotaOverhaul
{
    public class Config : SyncedConfig2<Config>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [SyncedEntryField] public static SyncedEntry<int> StartingCredits;
        [SyncedEntryField] public static SyncedEntry<int> StartingQuota;
        [SyncedEntryField] public static SyncedEntry<int> QuotaBaseIncrease;
        [SyncedEntryField] public static SyncedEntry<float> QuotaIncreaseSteepness;
        [SyncedEntryField] public static SyncedEntry<float> QuotaRandomizerMultiplier;
        [SyncedEntryField] public static SyncedEntry<int> QuotaDeadline;
        [SyncedEntryField] public static SyncedEntry<int> QuotaEarlyFinishLine;
        [SyncedEntryField] public static SyncedEntry<bool> QuotaEnablePlayerMultiplier;
        [SyncedEntryField] public static SyncedEntry<int> QuotaPlayerThreshold;
        [SyncedEntryField] public static SyncedEntry<int> QuotaPlayerCap;
        [SyncedEntryField] public static SyncedEntry<float> QuotaMultiplierPerPlayer;

        [SyncedEntryField] public static SyncedEntry<bool> CreditPenaltiesEnabled;
        [SyncedEntryField] public static SyncedEntry<bool> CreditPenaltiesOnGordion;
        [SyncedEntryField] public static SyncedEntry<float> CreditPenaltyPercentPerPlayer;
        [SyncedEntryField] public static SyncedEntry<bool> CreditPenaltiesDynamic;
        [SyncedEntryField] public static SyncedEntry<float> CreditPenaltyPercentCap;
        [SyncedEntryField] public static SyncedEntry<float> CreditPenaltyPercentThreshold;
        [SyncedEntryField] public static SyncedEntry<float> CreditPenaltyRecoveryBonus;

        [SyncedEntryField] public static SyncedEntry<bool> QuotaPenaltiesEnabled;
        [SyncedEntryField] public static SyncedEntry<bool> QuotaPenaltiesOnGordion;
        [SyncedEntryField] public static SyncedEntry<float> QuotaPenaltyPercentPerPlayer;
        [SyncedEntryField] public static SyncedEntry<bool> QuotaPenaltiesDynamic;
        [SyncedEntryField] public static SyncedEntry<float> QuotaPenaltyPercentCap;
        [SyncedEntryField] public static SyncedEntry<float> QuotaPenaltyPercentThreshold;
        [SyncedEntryField] public static SyncedEntry<float> QuotaPenaltyRecoveryBonus;

        [SyncedEntryField] public static SyncedEntry<bool> ScrapLossEnabled;
        [SyncedEntryField] public static SyncedEntry<float> ItemsSafeChance;
        [SyncedEntryField] public static SyncedEntry<float> LoseEachScrapChance;
        [SyncedEntryField] public static SyncedEntry<int> MaxLostScrapItems;

        [SyncedEntryField] public static SyncedEntry<bool> ValueLossEnabled;
        [SyncedEntryField] public static SyncedEntry<float> ValueLossPercent;

        [SyncedEntryField] public static SyncedEntry<bool> EquipmentLossEnabled;
        [SyncedEntryField] public static SyncedEntry<float> LoseEachEquipmentChance;
        [SyncedEntryField] public static SyncedEntry<int> MaxLostEquipmentItems;

        public Config(ConfigFile config) : base(Plugin.PluginGuid)
        {
            StartingCredits = config.BindSyncedEntry("Quota Settings", "Starting Credits", 300, "How much cash you want? \nVanilla: 60");
            StartingQuota = config.BindSyncedEntry("Quota Settings", "Starting Quota", 250, "The first quota. \nVanilla: 130");
            QuotaBaseIncrease = config.BindSyncedEntry("Quota Settings", "Quota Base Increase", 150, "The minimum quota increase. \nVanilla: 200");
            QuotaIncreaseSteepness = config.BindSyncedEntry("Quota Settings", "Quota Increase Steepness", 4f, "The quota increases exponentially, and this controls the steepness of the curve.  Bigger number means higher quotas. \nVanilla: 4");
            QuotaRandomizerMultiplier = config.BindSyncedEntry("Quota Settings", "Quota Randomizer Multiplier", 1f, "There is a bit of random variation each time the quota increases.  This number controls how much. \nVanilla: 1");
            QuotaDeadline = config.BindSyncedEntry("Quota Settings", "Quota Deadline", 3, "The number of days you are given to complete each quota.  \nVanilla: 3");
            QuotaEarlyFinishLine = config.BindSyncedEntry("Quota Settings", "Days Before Early Finish", 0, "The minimum number of days that need to pass before you are allowed to finish the quota.  Values lower than 0 mean that you're not allowed to finish early. \nVanilla: 0");
            QuotaEnablePlayerMultiplier = config.BindSyncedEntry("Quota Settings", "Enable Player Count Multiplier", true, "When enabled, the quota scales based on the number of players. \nVanilla: false");
            QuotaMultiplierPerPlayer = config.BindSyncedEntry("Quota Settings", "Multiplier Per Player", 0.25f, "The multiplier for each player.  Only applies to player counts above the threshold, and below the cap.  The formula is basically Quota * (1 + MultiplierPerPlayer * PlayerCount (adjusted based on the cap and threshold))");
            QuotaPlayerThreshold = config.BindSyncedEntry("Quota Settings", "Player Count Threshold", 2, "Only player counts exceeding this threshold will have an impact on the quota.  This option may be confusing, so here's an example: with a Threshold of 2 players (which is the default), if you have 2 players (including the host), the Player Count Multiplier will be 1, meaning the quota is unchanged.  Assuming the default Multiplier Per Player of 0.25, 4 players would give you a multiplier of 1.5");
            QuotaPlayerCap = config.BindSyncedEntry("Quota Settings", "Player Count Cap", 8, "Adding more players beyond this cap will not increase the quota multiplier.");

            CreditPenaltiesEnabled = config.BindSyncedEntry("Credit Penalties", "Enable", false, "When enabled, you will lose credits a percentage of your credits for every crew memeber that dies.  You can also enable dynamic mode, in which case the penalty is based on the ratio of dead players to alive players. \nVanilla: true");
            CreditPenaltiesOnGordion = config.BindSyncedEntry("Credit Penalties", "Enable At The Company", false, "When enabled, you also lose credits for dead players on Gordion/The Company. \nVanilla: true");
            CreditPenaltyPercentPerPlayer = config.BindSyncedEntry("Credit Penalties", "Penalty Per Player", 20f, "The percentage of your credits that you lose for each dead player.  Example:  Penalty Per Player = 20%, you have 1000 credits, 2 players die, you lose 400. \nValues >= 0");
            CreditPenaltiesDynamic = config.BindSyncedEntry("Credit Penalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on the portion of your crew that died.  This makes it more balanced for large crews.\nVanilla: false");
            CreditPenaltyPercentCap = config.BindSyncedEntry("Credit Penalties", "Penalty Percent Cap", 80f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
            CreditPenaltyPercentThreshold = config.BindSyncedEntry("Credit Penalties", "Penalty Percent Threshold", 20f, "If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving.  This applies to both the static and dynamic algorithms. \nValues between 0-100");
            CreditPenaltyRecoveryBonus = config.BindSyncedEntry("Credit Penalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies. A higher value means a higher incentive to recover bodies.  Applies to both normal and dynamic modes. \nValues between 0-100");

            QuotaPenaltiesEnabled = config.BindSyncedEntry("Quota Penalties", "Enable", true, "Increase the quota for each player that dies. Intended to replace losing scrap when all players die. Penalties are applied as a percent of the base quota for the round. Penalties only affect the current quota, and do not carry to future rounds. \nVanilla: false");
            QuotaPenaltiesOnGordion = config.BindSyncedEntry("Quota Penalties", "Enable At The Company", false, "Whether to allow quota penalties at the company building. \nVanilla: true");
            QuotaPenaltyPercentPerPlayer = config.BindSyncedEntry("Quota Penalties", "Penalty Per Player", 12f, "The amount to increase the quota per dead player, as a percentage of the base quota for this round. \nValues >= 0");
            QuotaPenaltiesDynamic = config.BindSyncedEntry("Quota Penalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on the portion of your crew that died.  This makes it more balanced for large crews.\nVanilla: false");
            QuotaPenaltyPercentCap = config.BindSyncedEntry("Quota Penalties", "Penalty Percent Cap", 50f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
            QuotaPenaltyPercentThreshold = config.BindSyncedEntry("Quota Penalties", "Penalty Percent Threshold", 15f, "If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving.  This applies to both the static and dynamic algorithms. \nValues between 0-100");
            QuotaPenaltyRecoveryBonus = config.BindSyncedEntry("Quota Penalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies.  Applies to both normal and dynamic modes.  For example:  Assuming a fully default coniguration, except without Dynamic Penalties, if you die, the penalty for your body is 12%.  If your body is recovered, 50% of the penalty is forgiven, leaving a 6% penalty. \nValues between 0-100");
            
            ScrapLossEnabled = config.BindSyncedEntry("Scrap Loss", "Scrap Loss Enabled", false, "If enabled, you will lose scrap when all players die.  (Dynamic scrap loss planned). \nVanilla: true");
            ItemsSafeChance = config.BindSyncedEntry("Scrap Loss", "Safe Chance", 25f, "A percent chance of all scrap and equipment being safe. When your items are 'safe', it overrides all other settings, and you keep everything. \nValues between 0-100 \nVanilla: 0");
            ValueLossEnabled = config.BindSyncedEntry("Scrap Loss", "Value Loss Enabled", false, "Lose a percent of total scrap value. \nVanilla: false.");
            ValueLossPercent = config.BindSyncedEntry("Scrap Loss", "Value Loss Percent", 100f, "The percentage of total scrap value to lose. \nValues between 0-100");
            LoseEachScrapChance = config.BindSyncedEntry("Scrap Loss", "Lose Each Chance", 50f, "A percent chance of each item being lost. \nValues between 0-100 \nVanilla: 0");
            MaxLostScrapItems = config.BindSyncedEntry("Scrap Loss", "Scrap Loss Max", int.MaxValue, $"The maximum number of scrap items that can be lost.");

            EquipmentLossEnabled = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Enabled", false, "Allow equipment to be lost. \nVanilla: false.");
            LoseEachEquipmentChance = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Chance", 10f, "A chance of each equipment item being lost. \nApplied after SaveAllChance. \nValues between 0-100 \nVanilla: 0");
            MaxLostEquipmentItems = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Max", int.MaxValue, $"The maximum amount of equipment that can be lost.\nApplied after EquipmentLossChance.");

            ConfigManager.Register(this);
        }
    }
}