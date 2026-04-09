using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;

namespace QuotaOverhaul;

public class Config : SyncedConfig2<Config>
{

    [SyncedEntryField] public SyncedEntry<int> StartingCredits;
    [SyncedEntryField] public SyncedEntry<int> StartingQuota;
    [SyncedEntryField] public SyncedEntry<int> QuotaBaseIncrease;
    [SyncedEntryField] public SyncedEntry<float> QuotaIncreaseSteepness;
    [SyncedEntryField] public SyncedEntry<float> QuotaRandomizerMultiplier;
    [SyncedEntryField] public SyncedEntry<int> QuotaDeadline;
    [SyncedEntryField] public SyncedEntry<int> QuotaEarlyFinishLine;
    [SyncedEntryField] public SyncedEntry<bool> QuotaEnablePlayerMultiplier;
    [SyncedEntryField] public SyncedEntry<int> QuotaPlayerThreshold;
    [SyncedEntryField] public SyncedEntry<int> QuotaPlayerCap;
    [SyncedEntryField] public SyncedEntry<float> QuotaMultiplierPerPlayer;

    [SyncedEntryField] public SyncedEntry<bool> CreditPenaltiesEnabled;
    [SyncedEntryField] public SyncedEntry<bool> CreditPenaltiesOnGordion;
    [SyncedEntryField] public SyncedEntry<float> CreditPenaltyPercentPerPlayer;
    [SyncedEntryField] public SyncedEntry<bool> CreditPenaltiesDynamic;
    [SyncedEntryField] public SyncedEntry<float> CreditPenaltyPercentCap;
    [SyncedEntryField] public SyncedEntry<float> CreditPenaltyPercentThreshold;
    [SyncedEntryField] public SyncedEntry<float> CreditPenaltyRecoveryBonus;

    [SyncedEntryField] public SyncedEntry<bool> QuotaPenaltiesEnabled;
    [SyncedEntryField] public SyncedEntry<bool> QuotaPenaltiesOnGordion;
    [SyncedEntryField] public SyncedEntry<float> QuotaPenaltyPercentPerPlayer;
    [SyncedEntryField] public SyncedEntry<bool> QuotaPenaltiesDynamic;
    [SyncedEntryField] public SyncedEntry<float> QuotaPenaltyPercentCap;
    [SyncedEntryField] public SyncedEntry<float> QuotaPenaltyPercentThreshold;
    [SyncedEntryField] public SyncedEntry<float> QuotaPenaltyRecoveryBonus;
    [SyncedEntryField] public SyncedEntry<bool> ChargeCreditsInsteadOfQuota;
    [SyncedEntryField] public SyncedEntry<float> CreditsPerQuota;

    [SyncedEntryField] public SyncedEntry<bool> VanillaScrapLoss;
    [SyncedEntryField] public SyncedEntry<bool> ScrapLossOnGordion;
    [SyncedEntryField] public SyncedEntry<float> ItemsSafeChance;
    [SyncedEntryField] public SyncedEntry<float> LoseEachScrapChance;
    [SyncedEntryField] public SyncedEntry<int> MaxLostScrapItems;

    [SyncedEntryField] public SyncedEntry<bool> ValueLossEnabled;
    [SyncedEntryField] public SyncedEntry<float> ValueLossPercent;

    [SyncedEntryField] public SyncedEntry<bool> EquipmentLossEnabled;
    [SyncedEntryField] public SyncedEntry<float> LoseEachEquipmentChance;
    [SyncedEntryField] public SyncedEntry<int> MaxLostEquipmentItems;

    public Config(ConfigFile config) : base(Plugin.PluginGuid)
    {
        StartingCredits = config.BindSyncedEntry("Quota Settings", "Starting Credits", 300, "How much cash you want? \nVanilla: 60");
        StartingQuota = config.BindSyncedEntry("Quota Settings", "Starting Quota", 250, "The first quota. \nVanilla: 130");
        QuotaBaseIncrease = config.BindSyncedEntry("Quota Settings", "Quota Base Increase", 150, "The minimum quota increase. \nVanilla: 200");
        QuotaIncreaseSteepness = config.BindSyncedEntry("Quota Settings", "Quota Increase Steepness", 4f, "The quota increases exponentially, and this controls the steepness of the curve.  Bigger number means higher quotas. \nVanilla: 4");
        QuotaRandomizerMultiplier = config.BindSyncedEntry("Quota Settings", "Quota Randomness Multiplier", 0f, "There is a bit of random variation each time the quota increases.  This number controls how much. \nVanilla: 1");
        QuotaDeadline = config.BindSyncedEntry("Quota Settings", "Quota Deadline", 3, "The number of days you are given to complete each quota. \nVanilla: 3");
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

        QuotaPenaltiesEnabled = config.BindSyncedEntry("Quota Penalties", "Enable", true, "Increase the quota for each player that dies. Intended to replace losing scrap when all players die. Penalties stack additively and are reset after each quota. \nVanilla: false");
        QuotaPenaltiesOnGordion = config.BindSyncedEntry("Quota Penalties", "Enable At The Company", false, "Whether to allow quota penalties at the company building. \nVanilla: true");
        QuotaPenaltyPercentPerPlayer = config.BindSyncedEntry("Quota Penalties", "Penalty Per Player", 12f, "The amount to increase the quota per dead player, as a percentage of the base quota for this round. \nValues >= 0");
        QuotaPenaltiesDynamic = config.BindSyncedEntry("Quota Penalties", "Dynamic Mode", true, "Instead of calculating the penalty as a flat rate per dead player, Dynamic Mode calculates the penalty based on the portion of your crew that died.  This makes it more balanced for large crews.\nVanilla: false");
        QuotaPenaltyPercentCap = config.BindSyncedEntry("Quota Penalties", "Penalty Percent Cap", 50f, "The percent penalty in the worst case scenario, all players dead and unrecovered. Any players still alive, and any bodies recovered (see Body Recovery Bonus) will reduce the penalty. \nValues >= 0");
        QuotaPenaltyPercentThreshold = config.BindSyncedEntry("Quota Penalties", "Penalty Percent Threshold", 25f, "If the penalty falls below this threshold, the penalty is set to 0. Increasing this value makes minor slip-ups more forgiving.  This applies to both the static and dynamic algorithms. \nValues between 0-100");
        QuotaPenaltyRecoveryBonus = config.BindSyncedEntry("Quota Penalties", "Body Recovery Bonus", 50f, "How much of the penalty to forgive for recovering bodies.  Applies to both normal and dynamic modes.  For example:  Assuming a fully default coniguration, except without Dynamic Penalties, if you die, the penalty for your body is 12%.  If your body is recovered, 50% of the penalty is forgiven, leaving a 6% penalty. \nValues between 0-100");
        ChargeCreditsInsteadOfQuota = config.BindSyncedEntry("Quota Penalties", "Charge Credits Instead", false, "Charges credits instead of increasing the quota.  You can set the conversion rate below.  Quota will only increase when you've run out of credits.  Creates an effect similar to Quota Rollover.  I encourage you to try this!");
        CreditsPerQuota = config.BindSyncedEntry("Quota Penalties", "Credits Per Quota", 1f, "The conversion rate from Quota Penalties to Credits.  Increasing this makes you lose more credits.  This can also be set below 1, to make credits less sensitive. \nValues: > 0");

        VanillaScrapLoss = config.BindSyncedEntry("Scrap Loss", "Vanilla Scrap Loss", false, "If enabled, scrap loss will work just like vanilla, or it can be handled by another mod.  The next option, Scrap Loss On Gordion, can override this.  So for truly vanilla behavior, set that to true as well. \nVanilla: true");
        ScrapLossOnGordion = config.BindSyncedEntry("Scrap Loss", "Lose Scrap on Gordion", false, "Toggles scrap loss at the company building.  If this is false, you won't lose scrap at the Company even if Vanilla Scrap Loss is true. \n Vanilla: true");
        ItemsSafeChance = config.BindSyncedEntry("Scrap Loss", "Safe Chance", 25f, "A percent chance of all scrap and equipment being safe. When your items are 'safe', it overrides all other settings, and you keep everything. \nValues between 0-100 \nVanilla: 0");
        ValueLossEnabled = config.BindSyncedEntry("Scrap Loss", "Value Loss Enabled", true, "If enabled, you lose a percentage of the total scrap value on board on a crew wipe. \nVanilla: false.");
        ValueLossPercent = config.BindSyncedEntry("Scrap Loss", "Value Loss Percent", 100f, "The percentage of total scrap value to lose. \nValues between 0-100");
        LoseEachScrapChance = config.BindSyncedEntry("Scrap Loss", "Lose Each Chance", 50f, "A percent chance of each item being lost. \nValues between 0-100 \nVanilla: 0");
        MaxLostScrapItems = config.BindSyncedEntry("Scrap Loss", "Scrap Loss Max", int.MaxValue, "The maximum number of scrap items that can be lost.");

        EquipmentLossEnabled = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Enabled", false, "Allow equipment to be lost. \nVanilla: false.");
        LoseEachEquipmentChance = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Chance", 10f, "A chance of each equipment item being lost. \nApplied after SaveAllChance. \nValues between 0-100 \nVanilla: 0");
        MaxLostEquipmentItems = config.BindSyncedEntry("Equipment Loss", "Equipment Loss Max", int.MaxValue, "The maximum amount of equipment that can be lost. \nApplied after EquipmentLossChance.");

        ConfigManager.Register(this);
    }
}
