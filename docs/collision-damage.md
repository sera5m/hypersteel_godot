# Collision damage

One rule: Jolt shoves first. HP is a **watch** that closes on **separation** (or timeout). Self-only packet. Never during the first interpenetrating frame.

- `CharacterBody3D`: open on `GetSlideCollision`, close when that RID is gone.
- `RigidBody3D`: open on `BodyEntered`, close on `BodyExited` (same Δv measure).
- Ballistic projectiles: `DoesBluntForce = false`. They use DamageCast / impact Area.

## Attach armor without fighting the capsule

Worn plates are **not** a second CharacterBody and are **not** Jolt-welded to the pawn.

- Pawn hull: layer PawnPhys only.
- Armor / HurtBox: `Area3D`, layer Damage, mask 0 (or Damage-only). `Monitorable` for traces. No collision response vs the capsule.
- Visual: `BoneAttachment3D` or child mesh. Stuck grenade: reparent + freeze move, do not add a joint.
- Joints (`PinJoint3D`, etc.) are for ragdoll / dragged props, not kit armor.
