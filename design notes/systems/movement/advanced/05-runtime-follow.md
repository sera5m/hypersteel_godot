# Runtime follow

Searcher is dumb. Kit is continuous.

```
BT / controller
  mask from useAdvMove*
  path = map_get_path(..., mask)
  follow Walk points with wish into Source
  if next corner is a Leap link:
      leap pulse → resume
  if next corner is on a WallRun region:
      WallRun.TryBegin()
      Integrate until region end or !IsActive
      JumpOff if metadata says kick
      resume Walk
```

## Rules

- `NavigationAgent3D` does not steer during kit lock (same gap as dash lock / GravityScale).
- Attach coyote is named and lives on `WallRunDef`.
- Same-wall regrab cooldown stays on the box, not the navmesh.
- Soldiers never get Wall/Leap bits; they cannot “discover” a wall by bumping it.

## Valkarie

Ignores these maps. Flight is open space + later air map. Do not bake a walk mesh she pretends to use.

## Tune order

1. Flags + empty Walk region (soldiers move).
2. Leap links + one leaper dummy + a fire obstruction.
3. One authored Wall region + Ryko dummy with `useAdvMoveWallrun`.
4. Auto wall split + smoothness costs.
5. Player `region_owns_point` as attach veto.
