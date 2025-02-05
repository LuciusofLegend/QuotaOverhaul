# QuotaOverhaul

The goal of Quota Overhaul is to make the quota system as fun, rewarding, and configurable as possible.  I've taken inspiration from several existing mods, added some stuff of my own, with some community input sprinkled on top.

**THIS PROJECT IS IN ALPHA**
QuotaOverhaul is not in a finished state, and there are currently several bugs that need fixing.  If you encounter a bug, check the github issues to see if it has been reported.  If not, please create an issue to let me know.

# Features

### Quota Variables
You can configure the starting quota, minimum increase, increase steepness and randomness multiplier (these are internal variables in vanilla).

### Player Count Scaling
The quota increases the more players you have.  This should make high player counts more balanced.  The scaling is always based on the highes number of concurrent players seen during that quota, to avoid exploits.  Very configurable.

### Configurable Credit Fines
You can toggle fines on and off, change the percent fine per dead player, or use the new Dynamic Mode, which is more configurable and scales based on the number of players online (specifically, the record player count on a given moon run).

### Increase Quota on Death
You get penalties to the quota for dying!  This is intended to replace losing all your scrap when all players die, and maybe the credit fines if you want.  Configurable with the same options as credit fines.

### Loot Saving
You can keep all your hard earned scrap, set a 50/50 chance for each precious piece loot, lose it all, or anything in between.

### Equipment Loss
You could also risk your handy equipment for an extra punishing experience.

# Roadmap

- Make an icon
- Dynamic Difficulty
- Dynamic Loot
- Dynamic Dungeon Size

Feel free to submit your ideas via a github issue!

# Known Issues

- Quota penalty doesn't scale with number of dead players properly
- The penalty menu looks weird
- Per-player quota penalty doesn't work right
- Items outside the ship don't despawn

If you find a bug, github issues are also great for that.

# NOTE:

Quota Overhaul is released under the MIT License, which means you are 100% free to fork it, copy it, modify it, and distribute it on any platform, under any license. If I ever stop maintaining this mod and it starts breaking with new versions of Lethal Company, I encourage you to fork it and keep the mod alive.

Happy Modding!

# Credits

Quota Overhaul takes inspiration from:
- Quota Tweaks by mrov
- Save Our Loot by MrHydralisk
- Custom Death Penalty by ImpulsiveLass