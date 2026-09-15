# DamageCast

Godot `IntersectRay` (hairline) or capsule `GetRestInfo` (tube). Mask = World | Damage.

Editor layers: 1 World, 2 PawnPhys, 3 Damage. HurtBox and ArmorPiece colliders stamp Damage in `_Ready`.

```
DamageCast.Ray(this, muzzle, aim, 200f, packet, shooterRid);
DamageCast.Tube(this, muzzle, aim, 200f, packet, radius: 0.04f, shooterRid);
```

Order: nearest hit. Armor `Hurt` soaks and forwards to the wearer (same instigator). If `LastOverpen`, the cast **stops on that plate**, then respawns from `entry + throughDir * ExitPad` so the same hull is excluded. Shrapnel recipe fires extra rays from the exit if `SpawnWorldIfExit`.

World without `IDamageable` eats the shot (bounce budget is 0 for now).
