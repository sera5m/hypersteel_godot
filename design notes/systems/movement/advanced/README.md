# Advanced movement — nav + kits

Setup problem, not a custom pathfinder.

Godot 4 `NavigationServer3D` layers + regions + links classify the level. Pawns expose capability flags. The same `WallRun` / leap verb the player uses executes the edge. Flight (Valkarie) stays off this map.

| File | What |
|---|---|
| [01-capability-flags.md](01-capability-flags.md) | `useAdvMove*` on the pawn, query mask |
| [02-nav-profiles-bake.md](02-nav-profiles-bake.md) | parse once, bake Walk / Wall / Leap into the level SETUP layer |
| [03-wall-regions.md](03-wall-regions.md) | vertical nav class, smoothness → cost, player can query it |
| [04-leap-links.md](04-leap-links.md) | 4 m linear gaps, fire as Walk obstruction |
| [05-runtime-follow.md](05-runtime-follow.md) | agent follows floor, hands the kit the link/region |

Runtime motion for walls: `Scripts/Abilities/Kits/WallRun/` on the Godot repo. Do not put gravity or attach coyote in the searcher.
