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


## Pocket verbs (minor enemy abilities)

Shared one-shot or short-duration flags any role can stamp. Not packs, not perks, not new Resolve. 0–2 per enemy. Tiny stamina/heat cost, long personal cooldown, readable from outside.

- **Shoulder check** — short dash that ends in a Blunt packet if they hit you or a wall. Scouts close; heavies shove off cover. Telegraphs with a half-step.
- **Drop and roll** — on a hard hit they go prone for 0.4 s and come up facing a random flank. Cheap anti-tunneling. Does not work if already critical.
- **Buddy tag** — marks the last person who damaged them for 3 s. Nearby allies treat the mark as soft priority. No outline, just better focus.
- **Stash and pull** — one extra magazine or stim on the hip. If they go empty or hit Medium they spend half a second to use it. Visible on the model.
- **Panic spray** — when a teammate dies in 6 m they dump a short uncontrolled burst in that direction. High friendly-fire chance, low accuracy. Conscripts only.
- **Fake death** — once per life, at Low HP they ragdoll for 2 s. If nobody finishes them they get back up with short LowVis. Execution still kills them.
- **Call the dog** — one-time radio ping that pulls the nearest cargo drone or autofense turret toward their position. Useless indoors or under jam.
- **Hot swap** — drop current gun and draw sidearm or melee in one motion when player closes to 4 m. Faster transition than a full reload, no animation cancel.

Rules:
- No time slow, no invuln, no extra damage packets beyond what the gun already does.
- Every one is a Status tag or a single TryPulse on a kit they already have.
- AI only fires them when a cheap condition is true (teammate died, magazine empty, player in 4 m, etc.). No new EQS.
- Players can see the telegraph or the model change.

## Implement order

0. Dummy packets.
1. Soldier + NpcBrain walk + last-known + AimHitscanTrack / AimLeadFlat (`22`). **LAND 2026-09-24:** SoldierPawn composes NpcBrain child with default soldier RoleDef; sense fills LastKnown from AssignedTarget export; EQS-lite behind-ray sets WorldHazardAhead so BackUp → GetOutOfView; Reflex.TryNadeLeap calls real JumpKit.TryPulse(Dash, away) then notes after fire. UseMeshConsensus remains false. No Valkarie tree.
2. Reflex nade-leap, log after. (done in 1)
3. HoldBand + range switch.
4. Terrified / Enraged.
5. Cover + BackOutOfView.
6. Scout / Lead RoleDefs.
7. TeamComms + rank. Mesh last.
