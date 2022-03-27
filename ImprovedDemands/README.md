## Improved demands
This mod changes the game's logic for which item a new store will demand. It works like this:

First, it determines a score for the currently supplied items, for each tier:
* Awful or Bad: 0 points
* Average: 2 points
* Good: 3 points
* Very good: 4 points

Then it determines the what is the highest tier that is currently being supplied (2 or more points). Call that tier N.
The store that spawns will be:
* Tier N+1 if tier N has at least 5 points, with probability equal to the points for tier N
* Tier N with probability equal to the points for tier N-1, plus the points for tier N if < 5
* Tier N-1 with probability equal to the points for Tier N-2 and lower.

This means while you are supplying enough of a tier 2 item, a city won't ever spawn a tier 0 store any more.
You can affect the chances by supplying more higher tier items, and fewer lower tier items.
Note that cities have a maximum number of stores based on population, 
so once a city grows it's probably a good idea to stop supplying low tier items. The store will disappear,
and eventually be replaced by a store with higher tier demand.

For stores in new cities, including those in your starting area, it's a little different:
Most stores will be Tier 0, but about one in five will be Tier 1.

The base game has a limitation that each city can have at most 2 stores of the same tier. This mod removes
that limitation, so that high tier stores will keep spawning even if you've reached endgame.
