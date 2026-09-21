# Enemy roster

2026-09-21 ingest. Law for campaign / MP meat, not a second damage game.

Sources used:

- Template `design notes/systems/enemy_data.txt` on `sera5m/hypersteel_godot` (format only).
- Listed Unreal names from the old dump title `ai_unts_old`: scout, soldier, squad leader, sniper, demonan, officer, combat medic, tech, engineer, plasma gunner, voltergheist, lasgunner, rocketeer, pyro, dog turret, drones, cargo transport drone, stubs.
- `16-ai-behavior.md`, `docs/damage-system.md`, `09-entity-perks.md`, `12-melee.md`, `13-guns.md`, `17-items-ammo.md`.

**The filled Unreal sheets were not on disk.** No `ai_unts_old.txt` in artifacts. `enemy_data.txt` is the blank frame plus a warning that it is old. Old HP / m/s / armor-tier integers are **unofficial** until that dump is pasted. Do not treat goal numbers below as locked.

Entry format follows the old frame: name, class, mass, speeds, glory-kill flags, brain, body, combat, style, abilities.

---

## Law (locked official)

Roles are **kits**, not classes. Same five boxes as operators: body, hip, pack, guns, perks. Same packets, same Status, same `TryPulse`. No AI-only packet. No `if (titan)` in Resolve. No new Resolve forks.

China-doctrine **meat / robot / cat medic** is a **stamp on the same role**, not three class trees.

| Stamp | What it actually is |
|---|---|
| **Meat** (China default infantry) | `Human` plan. Standard cyber+chems baked (`10`). Kar8 / Type 88 + Frag. Hip jumpkit or none. Feelings on. Morale on. Bleed legal. |
| **Robot** | `Machine` plan. Unfeeling default. Energy / plasma / rail lean. Fuel / Battery / Coolant sockets. No bleed. Amped not Stimmed. |
| **Cat medic** | `Cat` plan + **Stim pack** (or stim item) + Health kit. Inherent cat crumbs from `10`. Not a Medic class. Heal is `17` items / pack pulse, not a unique channel. |

Walkers stay `07`. Hydras are machines, not this roster. Named campaign spikes (Valkarie, Enduring, Redline, Arsonist, General, Sniper-as-boss) are **sheets on top of a role**, not extra classes.

### Grammar map (old → now)

| Old | Now |
|---|---|
| Generic AK / rifle | **Kar8-GL4V** or Type 88 family. Ammo name **Rifle** (`17`). |
| HP zoo | Occupant HP + **plate HP**. One magnitude split across segments (`10`). Packets, not a second bar. |
| Armor tier 0–3 | Plate **armorLevel** + attrs (`docs/damage-system.md`). Typical meat chest = Light (3) fabric or Plate (5) if they wore IV. Do not keep a parallel tier enum. |
| Glory-kill / execute | Crit **band** + `Band` / `Died` hooks. `Nuh uh` = 0 HP enters crit instead of Died. Second 0 dies. Executions still count. Not a QTE system. |
| Recover from glory kill | `Nuh uh` or `Spare heart`. Machines / drones cannot take Nuh uh. |
| Multiple glory states | Unofficial. Glory verb board is still open on `00`. Do not invent a second crit band. |
| Can be picked up | `12`: mass ≤ instigator Strength → lift. Mass ≥ **2×** instigator → grip, no lift. Grip ≥ **4×** theirs → struggle alone will not break. |
| Weak / strong / crit spots | Segments + **crit sockets** (Brain, Eye, WeaponHand, Fuel, Battery/Core, Coolant, LegActuator). |
| Damage resist table | Sheet multipliers after the layer walk. Not new Resolve. |
| Fear / rage | Morale (`Encourage` / `Terrified`) + official `Enraged`. Not Health. |
| Tick time | BT clock / perception tick. Not a damage stat. |
| Status names | Official tags from the damage doc only. `Powdered` = `ExplosiveCoated`. Magnet stays unofficial. |

### Compose flags (every roster pawn)

```
hasHealth = true
hasStatus = true
hasFeelings = meat / named / elites     // drones off
hasMorale  = meat + officer-led fireteams
critSockets = [Head]                    // machines add Fuel / Battery / Coolant
hasJumpKit / hasPack = sheet
```

Max 2 property perks + 1 hook perk (`09`). Drones skip feelings-hooks. Walkers skip bleed hooks.

---

## NpcAI implement first

Do not start Valkarie or a unique tree per name.

| # | Build | Done when |
|---|---|---|
| 0 | Dummy already takes packets through armor | exists (`NpcAI/Dummy`) |
| 1 | **Soldier** shoots, flinches, dies | Kar8 / Type 88 fire + `Damaged` → Feelings flinch + `Died`. No BT sophistication. |
| 2 | **Scout** alert | Perception ping. Sees / hears → sprint to last-known + call. Soft step. Does not invent a new packet. |
| 3 | **Squad leader** coord | Role verb “enclose.” Nearby meat take the call. Officer perk: friendlies ignore cheap taunt; leader death Terrifies them. |

After that: medic `TryPulse` stim on a marked ally, sniper height-and-stay, then the gun-role skins (plasma / laser / rocket / pyro) which are the soldier tree with a different right hand.

---

## Official vs unofficial

**Locked official**

- Kit-not-class. Same Resolve. Same items as `17`.
- Type 88 / named guns from `13`. Ammo names from `17`.
- Plate attrs + AP ladder from the damage doc.
- Glory = crit / Band / Died hooks. Nuh uh rules.
- Grab = mass × Strength from `12`.
- Cargo transport drone is **not** the standard drone turret base.
- Stubs stay stubs.
- China meat / robot / cat medic = stamps.
- Implement order above.
- No new Resolve forks.

**Unofficial (pending dump or later board)**

- Exact mass kg, walk / sprint m/s, turn deg/s, tick time from Unreal.
- Exact occupant / plate HP integers.
- “Can enter glory-kill state multiple times” as a third band.
- Magnet tag.
- Morale table numbers.
- Perception meters by species (cat hearing, robot cameras) — flagged open on `16`.
- Glory-kill as a full verb board (`00` still open).
- Any number written as a **goal** in the sheets below.

---

## Shared meat baseline (goal, unofficial numbers)

Use as a starting sheet so Soldier / Scout / Leader are the same pawn with different kits. Stamp robot or cat on top.

| Field | Goal |
|---|---|
| Plan | Human meat (cyber+chems on). Raw unaugmented is ~20% under (`10`). |
| Mass | ~90 kg kit. Lift-legal for Ryko. Grip-legal vs a heavy operator. |
| Height / width | Humanoid 1.7–1.9 m. Same biped as `10`. |
| Walk / sprint | Human Speed 0%. Jumpkit if the sheet has a hip. |
| Turn | Agility field. Conscripts sloppy. Elites track better. Center-stage lock, not aimbot (`16`). |
| Jump | Yes if `hasJumpKit` or the motor allows. Drones: sheet. |
| Glory recover | No, unless Nuh uh / Spare heart on the sheet. |
| Multi glory | No. |
| Pick up | Yes. Mass ≤ Ryko Strength. |
| Heal | Stim / Health kit from `17`. Medic ally can write the same. No AI-only regen magic unless a listed passive. |

---

# Filled roles

Each block is one **role kit**. Species stamp is a row, not a subclass.

Full sheets live in the project copy `artifacts/hypersteel/game design/21-enemy-roster.md` if this file is ever trimmed. This repo copy is the same law.

## Soldier
Rifle meat. Kar8 / Type 88, ammo Rifle, Frag. Feelings on. Crit legal, no Nuh uh. Pick up yes. First live NpcAI tree: shoot, flinch, die.

## Scout
Same pawn. Soft step. Sensor or cloak-drone pack. DP180 or Type 88. Alert role: see / hear → call.

## Squad leader
Coord verb enclose. Officer perk. Type 88. Death Terrifies the fireteam.

## Sniper
ISL-92c or Tri-gun. Height and stay. Close 0.6 s → Light pistol if the sheet cannot stay on the long gun. Roster sniper ≠ campaign Sniper mini-boss.

## Demonan
Explosive kit, name kept. Big Iron / Rockets / 40 mm. Send-off cook. Not a TF2 class.

## Officer
Morale stamp. Often the same pawn as leader. Officer perk. Delete them.

## Combat medic
Stim pack + `17` kits. Cat medic = Cat plan on this role, not a class.

## Tech
Sensor pack. Recon stick-drone. No hacker minigame.

## Engineer
Green / Trophy / Repair kit. Dog turret is a child pawn if they place one.

## Plasma gunner
Heavy plasma repeater. Plasma pellets. Thermal or Battery hip. Last word legal.

## Voltergheist
Arc rifle / Ion kit. Wetwired + bubble. Not a ghost. Not a new Status.

## Lasgunner
ISL-92c or Bipolar. Flak vs air. Mirror visor is a laser joke only.

## Rocketeer
Rockets ammo. Send-off. Rifle when the tube is empty or the target is inside 8 m.

## Pyro
Volcano + incendiary nades. Roster pyro ≠ campaign Pyro / Arsonist. Arsonist flame-burst break is a sheet flag (`12`).

## Dog turret
Child turret pawn. Unfeeling. Rip rules from `17`. Not infantry.

## Drones
Standard drone turret **base**. Unfeeling. Cloak drone is this actor for a scout pack.

## Cargo transport drone
**Unique. Not the drone turret base.** Vehicle mass. Child turret optional. Dump / flee. Grip like Redline’s bike — no clean yeet unless wrecked.

## Stubs
Exterminator, Crusher-as-AI, Surge: names only. Crusher MP card in `11` may be stamped onto the soldier pawn. Do not invent HP.

See the artifacts copy for the full brain / body / combat / style / abilities blocks per name.
