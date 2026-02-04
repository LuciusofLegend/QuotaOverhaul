namespace QuotaOverhaul
{
    public class DeathConsequences
    {

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
    }
}
