# Damage composition (how to use it)

See also `docs/damage-system.md` and `docs/entity-systems.md`.

## The Unreal interface, here

Do **not** cast the hit body to `RykoPawn` or `SoldierPawn`.

```
var actor = DamageProbe.FindEntity(collider as Node);
actor?.Hurt(packet);
```

`FindEntity` walks parents until `ActorEntity`. Mesh, plate, gun, `HurtBox` — all count.
`ActorEntity` implements `IDamageable`. That is the only type a projectile needs.

Put `HurtBox` on a limb collider and call `hurtBox.Receive(packet)`. It stamps segment/bone then walks up.

## Folder split

```
Scripts/Damage/          packet, AP ladder, armor data, status, feelings, kit, HurtBox
Scripts/Health/          HP state, bone map, HealthComponent
Scripts/Entity/Base/     ActorEntity door + kit compose
```

## Auto-compose

Assign a `KitSheet` on the pawn:

- `hasHealth` / `hasStatus` / `hasFeelings` / `hasMorale`
- sheets: `healthRules`, `armor`, `status`, `feelings`

`ActorEntity._Ready` → `KitComposer.Compose`. Missing child nodes are created. No reflection.

Soldier sheet: health + status + feelings.
Drone sheet: health + status, `hasFeelings = false`.
Ryko sheet: health + status + feelings; her `HealthRuleConfs` / `StatusRules` hold resist and heal rate.

## Hook ownership

- `HealthComponent` emits `Damaged` / `Died` only.
- `ActorEntity.OnHealthDamaged` tracks High/Medium/Low bands.
- `OnHealthBand` is empty on the base. `SoldierPawn` sets `Critical`.
- `FeelingsComponent` listens to `Damaged` only if the kit spawned it.

Projectiles never subscribe to those signals.
