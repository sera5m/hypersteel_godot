# Player ability board (hip / pack)

Locked shape 2026-09-16. Perks: `09-entity-perks.md` (sheet multipliers + existing hooks).
Does not edit `docs/damage-system.md`. Does not start Weapons or a GAS.

Campaign Ryko is **not** this board. She is a composed pawn with a baked JumpKit + scenes.
Multiplayer operators are limited humans / near-humans: one hip, one pack, guns. No Ryko pile-driver, no five-eye chassis.

---

## Segments (the useful idea from the old dump)

A loadout is four closed sockets, not an ability tree.

| Socket | What it is | What it is not |
|---|---|---|
| Pawn sheet | Body, healthRules, armor plates, resists | Class fantasy |
| Perks | Sheet multipliers + closed hooks. `09-entity-perks.md` | New verbs |
| Hip | One appliance. Movement / stamina / heat budget | A second pack |
| Pack | One appliance. The readable “kit” | Titan tactical + core + three grenades |

Abilities **come from the kit Resources**. `KitSheet.hasJumpKit` already works this way. Pack is the same pattern: `hasPack` + `PackDef`.

Do not:

- Subclass the player per kit (`SensorPilot : Operator`).
- Put pack logic in `PlayerMovement.cs`.
- Add verbs inside Resolve.
- Let campaign Ryko browse this menu.

---

## Hip (exactly one)

Replaces or is the jumpkit. All three are `JumpKit`-shaped boxes: a bar, pulse/hold, regen. Variants are defs, not new systems.

| Hip | Bar is | Pulse / hold | Trade |
|---|---|---|---|
| **Jumpkit** | Thrust | Air jump, dash, slam. Default MP hip. Same box as `Scripts/Abilities/Kits/JumpKit/` | No extra energy, no extra cool |
| **Battery / fuel** | Energy | Feeds pack verbs and energy guns. Weak or no dash. Can refill a teammate pack on use | You walk like a soldier unless the pack is a hover |
| **Thermal regulation** | Heat sink | Hold = dump heat (self `Vented`-like, opens no mech plates). Passive: slower weapon overheat, incoming Thermal cut | Little or no thrust. Energy guns stay up; you do not fly |

Rules:

- Hip is the only place dash lives unless a pack **adds** a `JumpVerb` flag (hover pack).
- Battery does not invent a second Resource type named Mana. It is capacity + regen on the same kit box, tagged `Fuel` if you want a crit leak.
- Thermal hip reads and writes **segment heat** on Status. No parallel heat meter.

Valkarie campaign: winged JumpKitDef (`hasFlight`) on her pawn. That is not an MP hip unlock; MP gets the hover **pack** if they want flight.

---

## Pack (exactly one)

One live appliance. Cooldown / charges live on the def. Shootable packs are child actors or extra plates; killing them drops or breaks the kit.

Packet / plate / Status only. If a pack needs a new Resolve branch, the pack is wrong.

### Sensor package (pulse blade →)

Passive

- Outline enemies through walls inside **2 m**.
- Smart-weapon lock **50% faster**.
- Cut incoming flash / muzzle interference (`Blinded` / `Deafened` duration down).

Active

- Reveal through walls: hostiles **≤ 2.5 m tall** inside **20 m**; larger silhouettes **+15 m** past that.
- Smart lock time → **20%** of normal while the pulse is live.
- Inside **5 m**: mines, satchels, cloaked actors as outline.

Implementation: perception / UI + lock multiplier on the weapon def. No damage packet. Size check is collider bounds or a sheet `silhouetteMeters`, not `if (titan)`.

### Cloak

- Pack deploys a **visible** drone that orbits you. You go invisible.
- Silent unless sprint or jumpkit pulse.
- If the drone is destroyed or away: plates **route light** — reduced vis to players and AI, not full cloak. This is a Status tag `LowVis`, not a second invis.
- Drone is an `ActorEntity` (health, no feelings). Shooting it ends full cloak.

### Hollo

- Up to **3** decoy drones. Follow look or a recorded route.
- Copy emotes. `/hollo say …` is a short VO/text emit from the decoy (MP).
- Touch or death → landmine packet (Explosive / Fragmenting). No friendly fire.
- Short-range radar ping, weaker than Sensor active.

Decoys are cheap pawns with a stick-detonate `ProjectileDef`. They do not run full Feelings.

### Stim / supercharger

TF2 Über + Titanfall stim, one verb.

- Massive speed + heal burst on **self** or a **designated humanoid** in range.
- Heal is HP write + `Stimmed` (already a Status tag: fight harder, bleed faster, stun/poison resist).
- Does not work on walkers / vehicles (body plan / no Flesh). Can work on FleshBorged.
- Battery hip makes this cheaper or longer. Thermal hip does nothing special.

### Grapple

- Titanfall grapple.
- Hold use + grapple a surface → **zipline** that dies if broken or you die.
- Motor: impulse along rope. Already a kit verb candidate; do not put rope math in SourceMove internals — pack calls `IMoveMotor`.

### Teleport pack (MP only)

Replaces phase. Campaign uncovers the tech; operators rent a short copy.

- Laser designator. **Cannot** pass walls. End point = aim point.
- Distance = base × charge above minimum.
- Telefrag: occupant at the point dies or takes a huge packet; **you take damage too**.
- Not in campaign loadout. Not Ryko.

### Plasma shield bubble

Helldivers bubble, Hypersteel materials.

- Volume of `PlasmaShield`: AP ≤ 5 kinetic dies in the skin. Lasers and DoTs **pass**. Plasma without ion **feeds** it. Lightning can pop it.
- Hold ability = **expand** radius (more plate HP drain / more heat).
- Double-tap = off.
- Replaces Amped Wall. No camping wall entity.

This is an external shield layer around the pawn, not a world prop. Juggernaut (below) is metal plates, not this.

**Feed vs charge (do not special-case Resolve)**

Same packet the gun already fires. The bubble is just a plate volume that already has these rows.

| You shoot the bubble with | Locked row | What the pack does with it |
|---|---|---|
| Plasma, no ion | Plasma **feeds** the shield | Restores bubble HP. Makes expand cheaper / last longer. No aura. |
| Lightning / Ion | Shield can **flicker or explode**. `Charged` = contact Lightning on bump, extra power budget, discharge hurts both | Intentional overcharge: expand radius from that extra budget. Overlap ticks Lightning on enemies. Owner eats a smaller contact tick too. Keep pumping past the cap → pop. |
| Laser | Lasers pass PlasmaShield | Does nothing for the bubble. Hits you. |
| Kinetic | AP ≤ 5 dies in the skin | Wastes ammo. |

The player-facing trick: lightning gun into your own pack → `Charged` → hold expand → walk an aura. Not a new channel. Wet still wins over Charged (cannot charge). Thermal hip cuts the self-tick and the heat cost of expand. Battery hip pays the gun so you can sit on the feed longer.

Enemy lightning is the same packet. They can charge your bubble for you or cook it off. That is the joke, not a bug.

### Hover / jump pack

Valkarie uses the campaign version.

- Adds `JumpVerb` Flight / Hover. Recharges the **hip** jumpkit fast while equipped.
- MP pack. Campaign Valkarie can ignore the menu and just have the def on her sheet.

### Autofense turret (“guard dog”)

- Two back-mounted guns, 360 scan.
- Priority formula on the def (air > near melee > low HP > look-aligned — tune later).
- Player picks the **gun type** (packet family), not a new Resolve.
- Low HP on the turret actors. Damage ≈ a rifle.

Child pawns. They use the same projectile door. No friendly-fire toggle in the first pass unless the lobby asks.

### Supply pack

- Ammo / battery / small heal to teammates on use.
- Boring on purpose. Exists so not every pack is a weapon.

### Portable bomb

- Hellbomb cousin. Bright blinker.
- **Lobby flag** because FF is the point.
- Packet: huge Explosive + Thermal. Fuel-socket walkers care.

### Juggernaut

- Directional shield pack. Upper torso + arms get **articulated SolidMetal** extras at vehicle rungs (armor **6–7**).
- Can be stripped (plate HP → Shattered / drop).
- Not a bubble. Not PlasmaShield. Ladder does the work; small arms Bounce/Null on those segments until the plates are gone.
- Look-direction matters: plates cover the front of those segments, not the back.

### Engineer

- Adds a **welder** to the weapon wheel (Cutting + Hot, short range, also repairs allied plate HP).
- Ability opens a build wheel: up to **3 turrets**, one shield wall, some mines.
- Shield wall is a placed `PlasmaShield` or MetalFoam volume, not Amped Wall camping if players can shoot the generator.
- Big. Quarantine behind “packs that spawn world actors.” Do not start this before Dummy HurtBox and a turret pawn exist.

### Utility drone

- Possess a drone: attack, slow heal / turret on owner when unpossessed, carry, help marked objectives.
- Ping wheel marks jobs.
- One drone actor. AI for the idle mode can wait.

### Radio control (open-sky only)

Pack is a radio + designator. The aircraft is a **world pawn** that can be shot down. Pack can be **jammed** (`Zapped` / local jam zone). Mostly useless indoors.

Submenu (all require sky):

| Call | Cadence | Notes |
|---|---|---|
| Strafe | Repeatable, cooldown | Eagle-style pass |
| Laser strafe | Repeatable | Laser channel; ignores plasma bubbles on the ground target |
| Rearm | Long | Resets the aircraft |
| Cruise micro-missile | ~once / 5 min | Weaker 500-kg cousin |
| Orbital lightning | Once / match | Needs sky. Lightning / Ion on mark |
| Orbital rail | Once / match | Vertical hypervelocity kinetic. AP through the column |

Also: request an AI squad, or drop a jam cylinder. Both fail under jam and indoors.

Do **not** hook this into campaign command. It is an MP sky map toy.

### Micro-missile pack

- 4 charges, 30 s each.
- Un guided shaped ADHPS-style rocket. 2–3 m splash. Spin-up before fire.
- Anti-armor packet (high AP kinetic + small Explosive). Not a lock-on.

---

## Conflicts / illegal pairs

Closed list, sheet-side, not Resolve.

- Hover pack + Jumpkit hip = intended (pack charges hip).
- Hover pack + Battery hip = flight with a fat energy bar, weak dash. Legal.
- Thermal hip + Plasma bubble = bubble lasts longer / eats less self-heat. Legal. Lightning-aura self-tick is smaller.
- Battery hip + Plasma bubble + lightning / plasma gun = intended feed/charge loop.
- Lightning-charging your own bubble is legal. Overcharge pop is legal. No `if (self)`.
- Cloak + Sensor active = Sensor still pings you to enemies who pulsed. Cloak is not Sensor-proof.
- Juggernaut + Plasma bubble = illegal (two full-body defense packs). Pick one.
- Radio + indoor map = pack disabled or charges frozen.
- Teleport + Hollo = legal; decoys do not teleport.
- Engineer + Autofense = two turret sources. Cap total spawned guns per player (say 5) so the map does not drown.

---

Avoid-list is in `design notes/README.md`. This file is the hip and the pack.

---

## Ship order if this ever becomes code

1. JumpKit hip def variants (battery, thermal) as Resource swaps.
2. Pack slot flag + one boring pack (Supply or Micro-missile) to prove compose.
3. Plasma bubble as a `PlasmaShield` volume on the owner.
4. Sensor as perception multipliers.
5. Everything that spawns extra pawns (cloak drone, hollo, autofense, engineer, radio air) after Dummy takes damage through a plate.
