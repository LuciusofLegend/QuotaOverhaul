# QuotaOverhaul

Configurable quota variables
Options to keep loot after all players die
Options to increase quota for dead 
Multipliers based on number of players for pretty much everything

## Quota Functionality
- Round 1, the quota is equal to startingQuota, or, if player multipliers are enabled, startingQuota * (multPerPlayer * (playerCount - playerCountThreshold)), where playerCount is equal to the number of players when the ship lands on the first moon.
- Once a quota is set, it cannot be reduced based on players leaving, but whenever a new player joins, the quota is increased for the rest of the round.
- At the end of each moon, if quotaDeathPenalties are enabled, the quota is increased based on the number of players that died, and optionally, if their bodies were recovered.
- When fulfilling a quota, before the new quota is calculated, the quota is reverted to what it would be without any player multipliers or death penalties.
- After the exponential functions are calculated, the quota is again multiplied by the number of players, and death penalties may accrue over the course of the round.
- Rinse and repeat.