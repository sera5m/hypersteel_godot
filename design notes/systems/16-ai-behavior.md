# AI behavior

Engine today: `Scripts/Entity/NpcAI/` — Dummy (packets work), Soldier (band hook + NpcBrain walk), empty Valkarie / SF stubs. Brain under `Brain/`.

Same kits as players. Same packets, Status, `TryPulse`. No `if (titan)` in Resolve. No AI-only damage.

Primitives (aim, nav, morale, traps, team) — `22-ai-primitives.md`. Leaves of these clocks. Not a second brain.

## Logic check

Four clocks on one pawn, plus sense and a reflex interrupt. Independent role rules first. Mesh later. Reflex logs after fire only.

## Layers

Sense, Reflex, Macro, Macro-micro, Micro-strat, Execution, Hooks (`Died` / `Damaged` / `Band`).

## Shared verbs

`MoveAndAttack`, `BackUp`, `GetOutOfView`, `Hold`, `Enclose`, `GoLook`, `ShootWorld`, `PulseKit`.

RoleDef reshapes range and idle. Scout close. Sniper height. Soldier mid.

## Implement order

0. Dummy packets.
1. Soldier + NpcBrain walk + last-known + AimHitscanTrack / AimLeadFlat (`22`). **LAND 2026-09-24:** SoldierPawn composes NpcBrain child with default soldier RoleDef; sense fills LastKnown from AssignedTarget export; EQS-lite behind-ray sets WorldHazardAhead so BackUp → GetOutOfView; Reflex.TryNadeLeap calls real JumpKit.TryPulse(Dash, away) then notes after fire. UseMeshConsensus remains false. No Valkarie tree.
2. Reflex nade-leap, log after. (done in 1)
3. HoldBand + range switch.
4. Terrified / Enraged.
5. Cover + BackOutOfView.
6. Scout / Lead RoleDefs.
7. TeamComms + rank. Mesh last.
