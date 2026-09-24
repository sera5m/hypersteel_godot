# AI behavior

Engine today: `Scripts/Entity/NpcAI/` — Dummy (packets work), Soldier (band hook only), empty Valkarie / SF stubs. No tree yet.

Same kits as players. Same packets, Status, `TryPulse`. No `if (titan)` in Resolve. No AI-only damage.

---

## Logic check (this stack is correct)

Four clocks on one pawn, plus sense and a reflex interrupt. Implementable. Poor wording does not make it a second game.

| You said | What it actually is | Trap |
|---|---|---|
| Macro / mesh consensus **or** independent role rules | Team intent. **Ship independent role rules first.** Mesh is optional later. | Building consensus before a soldier can walk. |
| Macro-micro | Shared verbs + EQS + world. “Back up” is a point. Wall → `GetOutOfView`. Barrel by target → shoot barrel. | Putting pathing inside this layer. |
| Micro-strat | **Role + kit** reshape the same verb. Sniper: height and stay. Scout: shotgun in. | A unique BT per roster name. |
| Execution | Feet, aim, swing, nade arc. | Aimbot / hard lock. Center-stage only. |
| Reflex | **Involuntary.** Dodge / counter / leap off nade. AI **notes it after it fires**, not before, not during. Not a strategy node. | Planning a dodge in the tree. |

Sensory: stats, emotion/morale (≠ HP), inventory, EQS. Then the four clocks. Leaves call `TryPulse` / fire / throw / path.

Projectile never talks to AI. `Damaged` → hook. Feelings flinch is optional (drones off).

---

## Layers

| Layer | Whose job |
|---|---|
| **Sense** | Stats, morale, inventory, EQS snapshot. |
| **Reflex** | Interrupt. Log after fire. |
| **Macro** | Role rule or (later) mesh order. |
| **Macro-micro** | Generic verb + world rewrite. |
| **Micro-strat** | RoleDef + kit shape the verb. |
| **Execution** | Path, aim, swing, throw. |
| **Hooks** | `Died` / `Damaged` / `Band` — same perk hooks. |

---

## Shared verbs (whole system)

`MoveAndAttack`, `BackUp`, `GetOutOfView`, `Hold`, `Enclose`, `GoLook`, `ShootWorld` (barrel), `PulseKit`.

RoleDef only changes *where* `MoveAndAttack` wants to stand and which gun range it likes. Scout shotgun close. Sniper elevated idle. Soldier volume mid.

---

## Implement order

0. Dummy already takes packets.
1. `NpcBrain` on Soldier: sense tick + `MoveAndAttack` execution stub (walk toward last known, no aimbot).
2. Reflex: nade-leap from incoming ordinance, log after.
3. Scout RoleDef: same brain, `GoLook` + ping.
4. Lead RoleDef: `Enclose` as a suggested verb; nearby meat may ignore if mesh is off.
5. Mesh consensus: only after 1–4 look alive.

Code lives under `Scripts/Entity/NpcAI/Brain/` — not inside SoldierPawn. Pawns stay thin hooks.

---

## Infantry / hull

Roles are kits (`21`). Hull uses the same brain, bigger sockets (`07`). Center-stage lock. Items from `17`.
