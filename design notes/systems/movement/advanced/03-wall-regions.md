# Wall regions (vertical nav class)

A wall-runnable patch is a real `NavigationRegion3D` whose “up” is the wall normal. AI pathfinds on it. The player can ask the same data “is this contact a valid wall?”

## Build

From parsed faces, keep those with `|n · WorldUp|` small (same idea as the parkour ShapeCast reject). Flood connected patches.

For each patch:

1. Average normal `N`.
2. Transform whose Y is `N`.
3. Bake a `NavigationMesh` in that frame with a *floor* slope (20–30° in wall space) so slight tilts stay and ceilings die.
4. Place `NavigationRegion3D` with that transform.
5. Layer bit `WallRun` only.
6. Store a `VerticalNavMesh` resource: mesh, `N`, patch id, smoothness.

## Smoothness → cost

`travel_cost` / `enter_cost` on the region. Search cost is why AI *picks* the face. Runtime speed stays on `WallRunDef.wishSpeed` (already above sprint).

| surface | travel_cost | note |
|---|---|---|
| glass / steel plate | 0.4–0.6 | wallrunners love it |
| painted concrete | 1.0 | neutral |
| brick / rivets | 1.4–1.8 | only if floor is worse |
| junk / pipes | no region | not runable |

Score = normal variance on the patch and/or a material tag (`smooth`, `panel`, `brick`, `pipe`).

Cheap region + fast kit = Titanfall reason-to-wallrun.

## Player pull

Player attach is still the hip/front volumes on `WallRun`. Second vote:

- closest point / `region_owns_point` on a Wall region
- or look up `VerticalNavMesh` by patch id

Painted glass, thin props, and damage-only colliders never get a Wall region, so they never attach even if a shapecast grazes them.

## Runtime

Tick attach every physics frame from volumes. Blend the *normal* over many frames. Project velocity onto the plane every tick (Source). Gravity scale 0.02 → 1 over attach time. Named attach coyote, not ground coyote.

Same box for L/R and face-on (`Side.Front`). Vertical FSM state is a stub.
