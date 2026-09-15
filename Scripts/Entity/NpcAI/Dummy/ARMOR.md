# Dummy + armor

`DummyPawn : ActorEntity`. Kit composes Health/Status. Parent an `ArmorPiece` under the dummy and give the plate a collider.

Call `DamageProbe.TryHurt(hitCollider, DamagePacket.KineticHit(50, ap: 3))`.
Probe picks the nearest `IDamageable` (the plate), plate soaks, remainder goes to dummy torso with the same instigator.

See `Scripts/Damage/ARMOR.md`.
