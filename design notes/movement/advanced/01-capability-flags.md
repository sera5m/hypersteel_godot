# Capability flags

Smart / advanced movers opt in. Grunts do not see walls or leaps.

## Flags

Same shape as `JumpKitDef.hasDash` — data on the sheet, not a subclass per pawn.

| Flag | Query layers | Who |
|---|---|---|
| `useAdvMoveWalk` | `Walk` | everyone grounded |
| `useAdvMoveLeap` | `Walk \| Leap` | jumpers, specialists |
| `useAdvMoveWallrun` | `Walk \| Leap \| WallRun` | Ryko, operators, rare elites |
| `useAdvMoveVault` | + `Vault` when that profile exists | same as wallrun set |
| flight | no mesh / later air map | Valkarie |

Path request:

```
mask = Walk
if useAdvMoveLeap    mask |= Leap
if useAdvMoveWallrun mask |= WallRun
if useAdvMoveVault   mask |= Vault
NavigationServer3D.map_get_path(map, from, to, true, mask)
```

If the bit is off, that region or link is invisible. No extra planner.

## Kit bind

Flags only answer “may I consider this edge.” Execution is the existing boxes:

- wall edge → `WallRun.TryBegin()` / `Integrate` / `JumpOff`
- leap edge → leap pulse (4 m linear, same motor surface as dash)
- vault edge → existing vault verb

AI and player call the same box. Controllers never steer `NavigationAgent3D` while a kit owns velocity.
