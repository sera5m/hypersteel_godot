# Projectiles

Terminology: in-game **wake cavitation** (not "super cavitation"). Flag is `KineticFlags.Hypervelocity` / alias WakeCavitation.

## What a projectile is

`ProjectileActor` is a cheap `IDamageable` flyer. It holds a **pointer** to `ProjectileDef` (shared Resource). Runtime copies only transform, velocity, owner. Do not clone the def over the net.

Destroyed by: `QueueFree`, turn into shrapnel rays, or `OnDestroyed` detonate (shoot the grenade).

On World/Damage hit the def may **stick** and/or **spawn** a scene (flame, decal). Instigator stays the shooter.

Game speed is authored. `FollowRealVel` only overrides **kinetic magnitude** from a placeholder J/cm²-ish function. AP / pierce stay on the def until a later pass.

## Hitboxes

- **Impact** (`Damage` layer): the bullet. Calls `DamageProbe` / `DamageCast` remainder.
- **Near-miss** (larger Area): whoosh if a pawn clips it. If wake-cavitation, that same area deals splash instead of whoosh. Near-miss does not deal damage unless that flag is on.

Combo table is a closed lookup (`ProjectileCombo`). Not an open type matrix every physics tick.

- Grenade × ballistic / hitscan / lightning → detonate
- Plasma bolt × lightning → swell
- Missile × lightning / hitscan / ballistic → midair explode
