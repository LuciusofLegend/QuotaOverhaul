namespace QuotaOverhaul
{
    public class DeathConsequences
    {

        public static void DoDeathConsequences(int deadBodies, int recoveredBodies)
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();

            int oldCredits = terminal.groupCredits;
            double creditPenalty = CalculateCreditPenalty(deadBodies, recoveredBodies);

            if (GameNetworkManager.Instance.isHostingGame)
            {
                terminal.groupCredits -= (int)(oldCredits * creditPenalty);
                if (terminal.groupCredits < 0) terminal.groupCredits = 0;
            }

            int oldQuota = TimeOfDay.Instance.profitQuota;
            double quotaPenalty = 0d;

            quotaPenalty = CalculateQuotaPenalty(deadBodies, recoveredBodies);

            if (GameNetworkManager.Instance.isHostingGame)
            {
                QuotaOverhaul.QuotaPenaltyMultiplier.Increase(quotaPenalty);
            }

            string penaltyAdditionText = $"CASUALTIES: {deadBodies}\nBODIES RECOVERED: {recoveredBodies} \n \nCREDITS: -{(int)(creditPenalty * 100)}% \n{oldCredits} -> {terminal.groupCredits} \n \nQUOTA: +{(int)(quotaPenalty * 100)}% \n{oldQuota} -> {TimeOfDay.Instance.profitQuota}";

            HUDManager.Instance.statsUIElements.penaltyAddition.text = penaltyAdditionText;
            HUDManager.Instance.statsUIElements.penaltyTotal.text = "";
        }

        public static double CalculateCreditPenalty(int deadBodies, int recoveredBodies)
        {
            if (!Config.CreditPenaltiesEnabled.Value ||
                !(Config.CreditPenaltiesOnGordion.Value || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion")) return 0;

            if (Config.CreditPenaltiesDynamic.Value) return CalculateDynamicCreditPenalty(deadBodies, recoveredBodies);
            else return CalculateStaticCreditPenalty(deadBodies, recoveredBodies);
        }

        private static double CalculateStaticCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Config.CreditPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.CreditPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Credit Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateDynamicCreditPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.GetRecordPlayersThisMoon() * Config.CreditPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.CreditPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.CreditPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Credit Penalty of {penalty}");
            return penalty;
        }

        public static double CalculateQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            if (!Config.QuotaPenaltiesEnabled.Value ||
                !(Config.QuotaPenaltiesOnGordion.Value || StartOfRound.Instance.currentLevel.PlanetName != "71 Gordion")) return 0;

            if (Config.QuotaPenaltiesDynamic.Value) return CalculateDynamicQuotaPenalty(deadBodies, recoveredBodies);
            else return CalculateStaticQuotaPenalty(deadBodies, recoveredBodies);
        }

        private static double CalculateStaticQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = Config.QuotaPenaltyPercentPerPlayer.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.QuotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
            }

            Plugin.Log.LogInfo($"Calculated Quota Penalty of {penalty}");
            return penalty;
        }

        private static double CalculateDynamicQuotaPenalty(int deadBodies, int recoveredBodies)
        {
            double penaltyPerBody = 1d / QuotaOverhaul.GetRecordPlayersThisMoon() * Config.QuotaPenaltyPercentCap.Value / 100d;
            double bonusPerRecoveredBody = penaltyPerBody * Config.QuotaPenaltyRecoveryBonus.Value / 100d;
            double penalty = deadBodies * penaltyPerBody - recoveredBodies * bonusPerRecoveredBody;

            if (penalty < 0 || penalty < Config.QuotaPenaltyPercentThreshold.Value / 100d)
            {
                penalty = 0;
                Plugin.Log.LogInfo($"Penalty fell below threshold of {Config.QuotaPenaltyPercentThreshold.Value / 100d}");
            }

            Plugin.Log.LogInfo($"Calculated Dynamic Quota Penalty of {penalty}");
            return penalty;
        }
    }
}
