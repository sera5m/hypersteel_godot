# Mech dump vs locked damage (curation)

Source: disordered old chassis / ability / perk list (Ashley / QuantumCroissant / vladddoi notes).
Do **not** edit `docs/damage-system.md`. That file is locked 2026-09-15.
This note is the same genre as `06-movement-health-from-old-doc.md`: mine goals, quarantine the rest.

Campaign lock that this dump keeps forgetting:

- Mechs are **spikes**, not the runtime. Operators are not a hero-shooter titan roster.
- Systems stay universal. Sheets + local hooks. No `if (titan)` inside Resolve.
- Perks are MP toys. Campaign is scenes.
- JumpKit is already a mounted appliance (`KitSheet.hasJumpKit`), not GAS.
- Heat, ion, plasma, laser, Coolant / Core / Fuel sockets already exist on the damage sheet.

If a line does not serve a sheet, a crit socket, a JumpKitDef, a grab verb, or a one-off campaign spike, it is the old doc talking.

---

## Verdict one-pager

Keep the **machine as a layered pawn** (reactor + vents + plates + heat budget).
Kill the **five playable titan classes with loadout screens**.
Translate player-scale “hip pack / backpack” into JumpKit + a second pack appliance.
Keep Valkarie’s aircraft as a vehicle pawn / winged kit, not a third movement system.

| Bucket | Fate |
|---|---|
| 5 chassis classes as a player roster | Quarantine. Enemy / vehicle sheets only. |
| Fusion core + coolant + vent weak points | Keep. Already Core / Coolant sockets + Thermal channel. |
| Heat recycler into melee | Keep. Thermal on the melee packet; self heat dump. |
| Plasma skin shield, stealable on rodeo | Keep. `PlasmaShield` external plate + grab yank. |
| Energy-vs-kinetic global rule from the dump | Partial kill. Locked doc already disagrees on lasers vs plasma shields. |
| Titanfall ability + core + perk trees | Quarantine for MP. Campaign uses scenes and glory-kills. |
| Hip pack / backpack (Helldivers-shaped) | Keep as compose flags. JumpKit is the hip. |
| Nuclear SSTO lore | World texture. Not a player verb. |

---

## How a “mech” is allowed to exist

Same stack as a soldier or Ryko.

1. Body plan subclass only if the silhouette changes (biped walker vs drone vs jet).
2. `KitSheet` flags: health, status, feelings off for pure machines, morale off.
3. Armor is plates + inherent segment armor at **vehicle rungs** (levels 6–11). Do not invent a parallel HP bar named “titan health.”
4. Reactor is a **Core** crit socket. Coolant loop is a **Coolant** socket. Fuel slosh is **Fuel**. Rear 10 kW bump is a small plate with low armor over Core, not a new component type.
5. Movement verbs ride `JumpKitDef` (dash count, charge-dash, hover, short flight) + `HypersteelPhysSauce` (mass, accel). Engine size is a def, not a class.
6. Occupant (if any) is a second `ActorEntity` or a hook. Rodeo / yank talks to the plate layer, not Resolve.

Ultra-Light through Ultra-Heavy become **five sheets**, not five games:

| Old name | Use | Armor ballpark | Motor |
|---|---|---|---|
| Ultra-Light (Reaper, long legs) | Rare recon walker / enemy spike | 6–7 | Fast sauce, wallrun flag, loud spark / heat tell |
| Light | Fast weapons platform | 7 | Light weapons only before speed tax |
| Medium | Default if a walker ever shows | 8 | Heavy gun = mild sauce down |
| Heavy | Campaign spike | 9–10 | Big Core, slow |
| Ultra-Heavy | Set piece | 10–11 | Biggest Core, almost no kit verbs |

Weapon-weight tax is PhysSauce / wish-speed, not Resolve.

Do not ship all five as player hulls. Campaign already said mech on/off in spikes. One loud light walker and one ugly heavy is enough to prove the sheets.

---

## Reactor / heat — map, don’t fork

Locked tools already cover this dump’s good reactor paragraph.

| Old line | Where it lives now |
|---|---|
| Portable fusion core, a few sizes | Core socket + sheet energy budget. Sizes follow chassis sheet, not a unique actor class. |
| Small rear 10 kW secondary, auto-vent, weak point above hips | Extra rear plate, armor 2–3, tagged Core. Venting applies `LeakingPower` / heat dump. |
| Activated thorium: +passive energy, +heat just by existing, +10% under load | Status on Core: constant low Thermal; load multiplies heat write. Energy budget field on the sheet. |
| Required coolant / heatsink / vents | Coolant socket. Passive heat bleed on Status tick. |
| Manual vent = faster cool, opens weak points | Status tag `Vented`: dump segment heat, drop vent-plate armor (or expose Coolant) for N seconds. |
| Heat recycler into melee (vladddoi) | On melee swing, if Core heat high: add Thermal channel to the fist packet, subtract heat from self. Burn only if heat is actually high. |
| Abilities raise a heat meter | Energy verbs write Thermal to Core / weapon segment. JumpKit bar can stay separate; heat is the machine tax, thrust is the pilot tax. |

Do **not** add a second “heat meter” UI system that ignores `StatusState` segment temp. If the HUD needs a bar, it reads Core segment heat.

Ultimatum / Rip-and-Tear / Ice-cold-blood are doomed or plate perks. They are sheet multipliers + a hook that ejects or pops Fuel when heat crosses ignition. They are not new Resolve bands.

---

## Shields, mines, pulses — packets and plates

The dump keeps inventing ability names for things the packet already does.

**Keep / translate**

- Skin plasma shield → `PlasmaShield` layer. Massive kinetic resist. Lasers and DoTs **pass** (locked). Plasma without ion **feeds** it. External lightning can explode it. HP bleeds fast. “Can be pulled off if rodeod” = yank the external plate (grab already in movement goals).
- Electric pulse, 3 charges → splash `Lightning` / `Ion` packet, short sphere. Destroys projectiles via combo table (missile × lightning already detonates midair). Does **not** need a new “blocks lasers” rule; lasers ignore plasma shields anyway.
- Point defense → child turret pawn, hitscan or laser packet at incoming `ProjectileActor`.
- Ion landmine / tether / ballistic tripwire / bury-for-mechs → `ProjectileDef` stick + trigger. “Mech not human” filter is **armor level or body-plan flag**, not a species check. Humans walk over a plate-pressure mine; walkers don’t.
- Electrified smoke / EMP smoke → area Status: `Zapped` + Wet-or-ion. EMP that blinds the user is an entity hook (Valkarie / walker specific), not Health.
- Repulse / sonic shock on rodeo → impulse + noise. Alerts AI. Not damage math.
- Enemy reactive armor → shaped splash packet from the plate when a layer dies. One-shot, then that plate is gone.
- Thermal skin (dump heat into chassis) → write a lot of Thermal into all segments; self-damage if already near ignition. Coolant gamble.

**Quarantine (fun, not now)**

- Rebound / vortex shield
- Delay shield (incoming damage → DoT until you heal)
- Projectile siphon into a coilgun
- Armor-theft hook as a dedicated titan ability (the **grab yank** already covers steal; don’t also ship a hook gun that is a second grab)

Delay shield in particular fights the packet: it would rewrite Resolve into a queue. If it ever ships, it is a Status that stores a deferred packet list, not a change to the ladder.

**Kill / rewrite the energy footnote**

Old note: energy does more to shields, no reload, heats you; kinetic does more to weak points, must reload.

Locked doc:

- `PlasmaShield` stops AP ≤ 5 kinetic dead. Lasers **ignore** it. DoTs pass. Plasma feeds it.
- Weak points are sockets + shattered / vented plates, not “energy is bad vs them.”
- Hot body: energy weapons stronger, own kinetic down. Cold: opposite + embrittlement on metal.

Keep: energy weapons heat the shooter (Hot / weapon overheat). Kinetic wants Core / Coolant / vent holes. Do not restore “laser melts titan shield” as a global.

---

## Abilities as verbs, not a class fantasy

Treat offensive / defensive / utility lists as **a toy box for later MP** and **one-off campaign props**. Do not stand up a titan loadout screen.

Worth stealing as verbs or props:

- Charge-dashes that collapse into one super-dash + 3 s lock → JumpKitDef (pulse stack / hold). Same box as pilot dash.
- Charge jump / short flight on light hulls → already `JumpVerb` Flight / Hover. Valkarie winged kit is the existence proof.
- Assault boost backblast → dash packet behind you (Splash + Thermal).
- Disarm at point-blank, same-or-lower class → grab + throw the gun actor. Weight check is mass on the sheet, not a class enum.
- Ticks / mines / barrage / lock missiles → weapons and `ProjectileDef`. Lock break = ion / speed on the target Status.
- Cloak drone that can be shot off you → child pawn + cloak tag. Firing pings sonar. 45 s redeploy is a pack cooldown.
- 360 vision / radar pulse / hologram / deceit-teamflip → UI / Status / AI perception. Deceit is multiplayer poison; keep it in the drawer.
- Phase → quarantine. You already roasted it in the dump.

Cores (Reactive blast, Death’s Kiss, Rearrange, Hailstorm, Eye of the Storm, Concentrated ion / Ramiel):

- Death’s Kiss / Rearrange / Rip-and-Tear are **glory-kill variants** on walkers. Entity hook. Vision cover is Feelings / a local camera hook.
- Hailstorm / lightning storm / giant laser are set-piece weapons or a boss verb (Valkarie / Long March), not a shared Core meter.
- Do not build Titanfall Core charge unless MP later demands a readable super. Campaign already has glory and scenes.

Anti-rodeo row (“none / emp / thermal skin / reactive / repulse / backup smoke”) is a **pack pick**, same slot as backpack. One at a time. “None” giving +health / faster recharge is a sheet modifier, fine for MP, skip in campaign.

---

## Weapons list — keep patterns, drop the titan roster

The light ☉ / heavy ◉ split is only a **wish-speed tax + hardpoint size** on a walker sheet. It is not two Resolve paths.

Patterns that already match the gun goal in `06-movement-health-from-old-doc.md`:

- Smart / marksman rifle, thermite, plasma charge, coil / rail, laser cutter, missiles.
- Dualies and tiny pistols are pilot guns, not walker mains.
- Harpoon that stacks slow → Status slow per attached actor. Fine as a pickup.
- Tesla / arc as an **ability splash**, not a primary (dump already said this).
- Pulse gun that “pierces all shields” → must mean **skips PlasmaShield** (laser-like) or it is a lie vs the locked plate. If it ships, give it Laser + Ion, not a “ignore armor” cheat.
- Heat axe / plasma lance / gauntlet blade → melee packets + Thermal. Consecutive-hit bonus is a weapon def, not Health.
- Flamethrower tight beam → Incendiary + stick rule (already: incendiary sticks when armor > AP).

Octipus nano-bomb spreader is a grenadier alt. Keep if the loadout stays ≤ six guns.

Do not author a full titan arsenal until plate-soak + HurtBox + pilot guns exist. Focus rule on the board still holds.

---

## Perks / augments / doomed / mechfall

Already locked: perks are MP toys.

If a later MP doc needs them, they are **sheet fields**, not new classes.

| Old pile | Legal form |
|---|---|
| Lightweight Alloys / Armored Plating / Spring Hydraulics / Ice cold blood / Steel wall | PhysSauce + healthRules + dash def + resistKinetic vs small-arms. Steel wall is mostly default: walkers sit at armor 6+ so small arms Null/Bounce already. |
| On Guard / Bloodthirst / Overclock / Reversal / Supercharge | Cooldown and Core-meter math. MP only. |
| Maglock / AP Rounds / Rampage / Tripod | Weapon def. AP Rounds is packet.ap, not a perk that rewrites the ladder. |
| Radar Ghost / Antinoise / Seventh Sense / Entrenchment | Perception / audio / UI. |
| Second Wind / Telophase / Final Stand / Ultimatum / Scavenge / Vengeance | Doomed / eject hooks on a walker entity. Ultimatum = remove heat cap, Fuel kaboom, auto-eject. Vengeance = Explosive + Ion splash + lingering toxic/radiation as Environmental packets. |
| Protective Fall / Portal In / Orbital Strike / Hitchhiker / HLC Cloak | Drop-in presentation. Dome is a shrinking PlasmaShield / high-resist volume, **not** invuln. Portal-in that cannot crush is the safe default. |

`Steel wall` “mechs take −80% from small arms by default” is redundant if armor levels are honest. Prefer the ladder. Don’t stack a secret global mul on top.

---

## Aircraft

Valkarie’s jet is canon. It is a **vehicle pawn** with a huge Core (lore: ~5× Ryko chest), plasma-poor China vs Japan compact plasma as flavor, and `JumpKitDef` with `hasFlight`.

Nuclear SSTO / scramjet-heated-by-fission is world texture for how expensive orbital craft are. Do not simulate reactor-scramjet thermodynamics in movement.

Pilot-scale: you are not flying that jet in the default loop. Campaign uses it as a boss silhouette / arrival / escape.

---

## Pilot scale — the actual gem

Hip pack + backpack, Helldivers-shaped, JumpKit already exists.

```
hip  = JumpKit (thrust, dash, slam, optional hover)
pack = second appliance slot (fuel, cloak drone, extra plate, ammo well, pyro canister, anti-rodeo)
```

Rules that keep this from becoming GAS:

- Closed flag list on `KitSheet` (`hasJumpKit`, later `hasPack`).
- One live box per slot. Variants are Resources, not subclasses per character.
- Pack does not get a full ability tree. It is one device with a cooldown / fuel.
- Pyro-style fuel canister is a pack that writes Incendiary on a tool and LeakingFuel on crit.
- Enemies can wear packs. Shoot the pack off = drop the device actor.

This is the translation of “we want to rip Helldivers backpacks,” not a titan tactical.

---

## What not to build from this dump

1. Playable Ultra-Light→Ultra-Heavy roster with shared hardpoints and a loadout UI.
2. A second heat system next to segment temp.
3. “Energy melts shields / kinetic hates shields” as a global opposite the `PlasmaShield` row.
4. Titanfall Core meter + execution-as-core as campaign structure. Glory-kills already cover the fantasy.
5. Phase, deceit-teamflip, 360-FOV as default verbs.
6. Any `if (mech)` inside `HitCalculations.Resolve`.
7. Starting Weapons or AI combat work to serve this list. Board focus is still plate-soak + HurtBox on Dummy.

---

## If something from this dump ships next

Only these are cheap and legal against the locked docs:

1. Walker sheet: armor 7–9, no feelings, Core + Coolant sockets, `hasJumpKit` with a fat dash def.
2. `Vented` status that cools Core and drops vent-plate armor.
3. Melee heat recycler: Thermal on fist, heat down on self.
4. External `PlasmaShield` plate that grab can steal.
5. Pack slot design note when JumpKit compose is no longer `wip`.

Everything else stays in this file until MP or a named campaign spike asks for it by name.
