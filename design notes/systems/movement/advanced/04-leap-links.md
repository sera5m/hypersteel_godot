# Leap links

Leap = linear jump across a gap the Walk mesh will not span. Default gap **4 m**. Not a third physics sim.

## Build

After Walk bake, walk region rims. Pair points if:

- horizontal distance ≤ 4 m
- height delta within jump height
- air ray between them is clear

Emit `NavigationLink3D` (two-way, or one-way down). Layer `Leap`. Travel cost ≈ time-in-air, cheaper than walking a long burn.

## Hazards

Fire, acid, wrecks = `add_projected_obstruction` on the **Walk** source/mesh only.

Leaper query is `Walk | Leap`, so A* leaves the floor via the link instead of walking the hazard. That is `.useAdvMoveLeap`.

Do not stamp the same obstruction onto Wall or Leap — those exist so the searcher can leave the floor.

## Execute

On link reached: disable agent steering, pulse leap on `IMoveMotor` (wish = link tangent, ~4 m), then resume Walk. Same “kit owns velocity” rule as wallrun / dash.

Ground leap to skip floor threats is allowed anywhere two Walk points would have accepted a 4 m link. Author extra links near authored fire if the auto rim pass misses the angle.
