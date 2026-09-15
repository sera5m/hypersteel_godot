# Collision damage

Jolt (Godot physics) **moves** bodies and reports contacts. It does **not** deal HP. We read the contact and emit a blunt `DamagePacket`.

- Heavy slow prop vs pawn: crush if `otherMass / pawnMass` is large, even at low speed.
- Two damageable actors: reduced-mass impulse, efficiency < 1 (default 0.65 head-on). Glancing uses the contact normal (same idea as movement rebound).
- Rocks, thrown melee, corpses: same path if they have mass + `CollisionDamage` or hit a pawn that has it.
- Bullets stay on `DamageCast` / projectile impact. Do not run this on `ProjectileClass.Ballistic`.

Pair cooldown stops both sides applying the same smack twice in one frame.
