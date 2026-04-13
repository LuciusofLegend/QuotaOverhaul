## 2.0.1

- Fixed scrap loss handling, which was totally incorrect. Not sure how I missed it in testing but whatever, it should be good now.

## 2.0.0

- Updated to work with v81!
- Added a new option under Quota Penalties, where you are charged credits for each player that dies, and when you've run out of credits, an any credits that you can't pay because you've run out, are converted to an increase in the quota.  You can configure the conversion rate.  This was a community requested feature, so give it a shot!
- Added a new option for scrap and equipment loss where you can set a maximum percentage of the items on board, and you will never lose more than that.
- Added a new option to toggle scrap loss on Gordion/The Company
- Removed the option to have an alert showing lost scrap
- Updated the default configuration
- Renamed options to hopefully make them more clear
- Updated config descriptions to hopefully be more clear
- Variables that are set to their vanilla values no longer force those values.  This should increase compatibility with other mods.  For example if you want another mod to handle the Deadline setting, you can set it to it's vanilla value of 3.
- Did some internal refactoring
- Added a lot more logging, which should make future testing easier
- Improved the README a bit

## 1.4.6

- Made the lost items alert optional and disabled by default

## 1.4.5

- Fixed a dependency issue that caused the mod to not launch at all
- Updated the README and description

## 1.4.4

- Did some more testing, fixed some minor oversights, but most things already worked as far as I can tell.
- Added the Font Update mod as a dependency, to fix the weird looking + signs.

## 1.4.3

- Revisited the project after 6 months of inactivity (yay)
- Got things to a point where they mostly work, but didn't thoroughly test for bugs.

## 1.4.0

- Fixed a bunch of issues with belt bags
- Changed the "Loot Saving" configs to "Loot Loss". I did this because it makes the code and option names more consistent. You should delete your config and relaunch before making edits. Read the config descriptions so you know what has changed. Sorry for the inconvenience.

## 1.3.1

- Fixed some interesting bugs with loot despawning, which were caused by a misnamed patch function

## 1.3.0

- Fixed some syncing issues with the Quota Penalty

## 1.2.6

- Make most things only run host-side.  Should hopefully fix a bug or two.

## 1.2.5

- Fixed the quota being stuck at 0

## 1.2.4

- Added an option to set the earliest day the quota is allowed to end
- Added an icon by Endoxiom

## 1.2.1

- Fixed the Quota Penalty always being zero when using Dynamic Mode
- Added state saving

## 1.2.0

- Fixed props not despawning when leaving a moon
- Attempted a fix for quota penalty issues (untested)
