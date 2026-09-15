# ArmorPiece

Armor is its own `IDamageable` node, not a row on Health.

- Parent = wearer `ActorEntity`.
- Mount = `BodySegment` (arm, chest…). Through-damage goes to that segment.
- Instigator stays the bullet/fist. The plate is cover, not the attacker.
- Collider + mesh are visual/hitbox only. Swap the box per segment. Attributes live on `ArmorPlate`.
- `comAxis` = long axis of the plate. Bounce uses that + hit point (`DamageReceive.BounceDirection`).
- Signals — `ArmorDamaged`, `ArmorShattered`, `ArmorBroken`, `ArmorRepaired`, `ArmorBounced`. Cause node is the actor that did it (or the repairer).
- `DamageWatch` is event-updated (`Time.GetTicksMsec()`). Enable `watchEnabled`. DoT is summed until `ConcludeDot()` (effect ended or plate HP ran out). Not per tick.

Shared math is `DamageReceive` + `ArmorLadder` in this folder. Crit sockets are the inverse (`CritSocket`) — inside the body, mul + optional tag.

Test — `Scripts/Entity/NpcAI/Dummy/DummyPawn.cs`. Scene: DummyPawn root, Health via kit, child ArmorPiece with an Area3D/CollisionShape3D. `DamageProbe.TryHurt(collider, packet)` hits the plate first.
