# Hypersteel entity and systems design

Locked 2026-09-15. Systems stay consistent. Variants are data and local hooks, not forks of the system.

## Layers (always this order)

1. **System** — one function for the whole game. No pawn names.
   Examples: `HitCalculations.Resolve`, `HealthState.Apply`, `SourceMove.Accelerate`.
2. **Universal config** — project defaults on the system or a shared Resource.
3. **Actor sheet** — Resource on the pawn (`HealthRuleConfs`, `HypersteelPhysSauce`, later `FeelingsRules`).
   Missing fields fall back to universal config.
4. **On-event hooks** — implemented on the **entity class**, not inside the system.
   Observers are local. A drone ignores a health-band change. A soldier uses it to enter critical.

Entity is a socket strip. Inheritance names the socket layout (biped, drone, boss kit). Composition is the appliances (health, motor, feelings).

`ActorEntity` **requires** slots (health, motor). It does not implement armor math or Source accel.
The inheriting actor **picks** the controller (`PlayerMovement` vs AI motor) and the sheets.

## Inheritance vs composition

Use a subclass when the **body plan** changes (biped vs drone vs turret).
Use a sheet + components when numbers or optional reactions change.

Do not:
- `if (entityId == "ryko")` inside Health or Move
- put `TakeDamage` overrides that replace `Apply`
- grow `SpecialForcesSniper : SpecialForces : Soldier` for loadout variants

A new grunt is a `.tres` and maybe one hook. Ryko and a soldier share `HealthState`. Ryko’s sheet says she does not take debuffs the same way.

## Health thresholds — entity-local

`HealthState` reports HP / segment / result. It does **not** decide “medium → low.”

The entity subscribes locally:

- On `Damaged`, compare total or torso HP to its own bands.
- If it crosses a band, the entity runs its own reaction.
- Drone: empty hook.
- Soldier: enter critical / fallback behavior.
- Ryko: whatever her pawn implements.

Thresholds are useful. They are not a global observer bus.

## Flinch / feelings — compose by property

Flinch is **not** hard-wired into Health.

Any enemy with the attribute **has feelings** composes a feelings/flinch component (or a flag + rules struct on the entity).
Enemies without it do nothing after a hit except lose HP.

Influence on a flinch (entity variables, not system special cases):

- `durability`
- `attackRelativeHealthLossPercent` (this hit vs current/max HP)
- `resistance`

Health still only emits `Damaged(result)`. Feelings reads `result` + those fields and decides whether to flinch. Automatic flinch is for bodies that opted into the property.

## Consistency test before adding a class

1. Same function + two Resources? → no new class.
2. Different driver (player vs AI)? → strategy in the motor slot.
3. Different body plan? → one subclass.
4. Story / VO / one-off beat? → hook on the concrete pawn.
5. About to branch on a name inside a system? → stop; sheet or local hook.

## Folder reminder

- `Scripts/Health/` — system + sheets + `HealthComponent`
- `Scripts/Entity/Base/` — `ActorEntity` shell
- `Scripts/Entity/PlayerChars/Ryko/`
- `Scripts/Entity/NpcAI/{Bosses,Soldiers,SpecialForces}/`

Movement stays `SourceMove` + sauce + FSM verbs plugged into the motor slot.
