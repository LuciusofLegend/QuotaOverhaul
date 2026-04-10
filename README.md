# Quota Overhaul (v81 Compatible)

Quota Overhaul aims to be your one-stop-shop of balance changes related to the quota system.  The consequences of death are heavily tied to the quota from a game design perspective, and therefore Quota Overhaul also focuses on that area.  This mod is extremely configurable.  You can enable all the features, or just the ones you like, and you can decide exactly how they will behave (within reason).

By default, Quota Overhaul makes the following changes to quota balance and the general game loop.  These changes reward cautious play early on, and encourage risk taking when you're struggling to meet the quota.  It makes individual deaths more consequential, and makes team wiping less devastating:

You start with 300 credits instead of 60  
In a solo run, the quota starts at 200  
For each additional player, the quota increases by 25%  
The quota will rack up slightly faster  
There is no randomness applied to the quota  

You don't lose any credits for dead players  
You don't lose any scrap on a team wipe  
Instead the quota increases for each player that dies, proportional to the number of players in the lobby  
Collecting bodies decreases the quota penalty  

I may add a proper features list at some point, but I'm finding it hard to describe them right now, since everything is so configurable it can get nebulous.  I would recommend loading up Lethal Config and looking through the options.  They all have descriptions that hopefully will help make everything clear.  It can be a rabbit hole tho ^_^

# Mod Synergies

Here are some mods I like to use with Quota Overhaul to complement the balance changes by providing more risk/reward mechanics:

### [Sell Bodies Fixed](https://thunderstore.io/c/lethal-company/p/Entity378/SellBodiesFixed/) by Entity378  
Sell Bodes allows you to sell the corpses of slain entities to the company.  It provides a very fun alternative game loop of monster hunting, which leads to a lot of funny moments.  

### [Weather Tweaks](https://thunderstore.io/c/lethal-company/p/mrov/WeatherTweaks/) + [Weather Registry](https://thunderstore.io/c/lethal-company/p/mrov/WeatherRegistry/) by mrov  
Makes bad weather into a tradeoff by providing a boost to scrap value on a moon based on its weather.  Great risk, but also great rewards.  That's the change relevant to Quota Overhaul anyway, but there are even more great weather related features in these mods.  Definitely check them out.  

### [Lethal Casino](https://thunderstore.io/c/lethal-company/p/mrgrm7/LethalCasino/) by mrgrm7  
I like this mod because when things are going badly, there's a tendency to want to give up.  But if you know there's always a chance of hitting it big at the Casion and beating the quota, you'll keep trying to the very end!  Which means more fun with your friends (if you have those), and less dwelling on the prospect of losing. 


# Roadmap

- More options of quota algorithm, for example to have it cap out at a certain point or severely limit exponential growth
- Daily Quota compatibility

# Known Issues

Probably incompatible with any mod that changes the quota.  With certain configurations it might work, but generally Quota Overhaul will overwrite any changes.

# Acknowledgements

- Thank you to mrov for making Quota Tweaks, which I stole code from
- Thank you to MrHydralisk for making SaveOurLoot, which I also stole code from
- Thank you to ImpulsiveLass for making CustomDeathPenalty, which inspired the Quota Penalties feature
- Thank you to Endoxicom for making the icon

# Licensing:

Quota Overhaul is released under the GPLv3 license.
This means you are free to distribute your own version, provided you release it under the same license.
For the moment, if you encounter issues with the mod I encourage you to open an issue on github so I can fix it,
but if I stop maintaining the mod and it starts breaking on new game versions, feel free to make a fork.
