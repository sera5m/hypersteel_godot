# Entity perks + kits on AI

2026-09-16. Does not edit `docs/damage-system.md`.
Perks are sheet flags. Kits are the same hip/pack Resources players use.
Complex AI is **BT goals calling `TryPulse` / `SetHold` on a kit the grunt already composed**, not a new pawn class per behavior.

Campaign named freaks (Ryko, Valkarie) can wear these as numbers. They still do not browse the MP menu.

---

## Two rows only

| Kind | Allowed | Not allowed |
|---|---|---|
| **Property** | Multiply a field that already exists (sauce speed, plate HP, cooldown, heat write, lock time, flinch resist, heal rate) | New buttons |
| **Hook** | Local react on `Damaged` / `Died` / `BandChanged` / kit `TryPulse` | New Resolve bands, `if (perkName)` inside Health |

Operator map (`10-mp-cosmetic.md`): segment = stats, **core** (chest) = one perk from this file, **training/HUD** = one Habit, hip/pack = buttons.
If it needs a key press, it is a hip or a pack (`08-player-ability-board.md`).
Stack rule same as plate attrs: at most two perk rows that touch the same field; second copy is half.

Most pawns can wear 0–2 property perks and 0–1 hook perk. Drones skip feelings-hooks. Walkers skip bleed hooks.

---

## Kits on AI

Same compose as operators.

- Grunt sheet: `hasJumpKit` and/or `hasPack` + defs.
- BT does not own dash math. It asks `CanPulse(Dash)` and fires `TryPulse`.
- Sensor pack on a spotter is why they call you through a wall at 20 m.
- Cloak drone on a sneak is why the drone is the thing you shoot.
- Stim pack on a medic is Über for a marked fireteam, not a unique Medic class.
- Plasma bubble + lightning gun on an enemy is the same feed/charge/pop loop as a player. They will do the funny thing on purpose.

That is the “complex ass behavior.” Perception + kit verbs + morale. Do not grow `SpecialForcesSniper : SpecialForces : Soldier` to fake it.

---

## Locked list (yours, mapped)

### Overclocked — property
+15% ability recharge, +15% move speed, +15% overheat rate, +15% stamina / thrust spend.
Hot-rod. Burns the bars to live in them.

### Send-off — hook (`Died`)
After death, munitions still on the body (grenades, missiles, Fuel leak, stuck incendiary) start a loud beep, then cook off as their normal packets.
No extra explosion type. If they were empty, they just beep and embarrass themselves.
Walkers with Fuel / Coolant sockets are the loud version.

### Nuh uh — hook (`Died` / band)
HP hitting 0 enters the entity’s **crit band** instead of `Died`. No `Bleeding` out.
This is the soldier Critical hook with the bleed clock off. Drones and raw machines cannot take it (no crit performance). A second 0 after crit is real death. Executions still count.

### A thermodynamic pact — property + hook (`TryPulse`)
Ability uses unlimited for practical purposes, cooldown −90%.
Each use writes heat to **full** on the hot segments. Next use while already at cap: `Burning` + HP that will not take pack/stim/siphon heal (`heal mul` 0 on that damage).
Not a new health pool. Status already has heat ignition and toxic-as-heal-cut. Pact is those two dials at 11.
Thermal hip is the coward’s version. Pact is the joke.

### Thick armor — property
+30% plate / segment HP. One extra plate attribute slot (still max two copies of the same attr, second half). −20% speed.
The extra attr is chosen on the sheet (Ceramic, Composite, ShearThick, etc.). Perk does not invent a material.

### Too light to hit — property
+30% speed, −30% plate HP, +10% ability recharge.
Opposite of Thick. Incoming AP ladder unchanged; you just have less plate to spend.

### Brawler — property + hook (`Damaged` on other / kill notify)
Small HP on landing a melee packet. Larger HP on a kill you owned, extra if the kill was melee.
+25% melee rate (weapon def / windup).
Uses the same kill-to-heal pipe as Ryko’s loop, tiny numbers. Not glory.

### Ice cold — property
−10% cooldown, less heat buildup (incoming and self-write), −10% speed.
Thermal hip is the appliance. Ice cold is the pawn that was already like that.

### Bounce back — hook (Feelings ragdoll)
When the pawn would go `Ragdolled`, skip the flop and fire an `IMoveMotor` impulse instead (look-away + up, speed from the hit).
Heal rate up while that launch is live.
Jumpkit / hover / slide-bounce read this as free height. Heavy boots deny it (both want the same flinch event).
Executions and cavitation still pin. This is not a get-out-of-gib.

---

## More in the same grain

Property unless marked hook.

| Perk | What it touches | Why it exists |
|---|---|---|
| **Soft step** | Move noise −80% (audio / AI hear radius) | Antinoise without a pack |
| **Clatter** | Opposite. Hear radius up. Shot scare on nearby conscripts | Cheap tell. Pairs with Send-off |
| **Sandbag** | Flinch / flash / stun duration down | Feelings resist. Drones ignore |
| **Marked** | Incoming smart-lock faster. Sensor treats you as 2.5 m even if you are smaller | The guy everyone sees |
| **Loose plates** | Less incoming shrapnel; on `Died` spawn more fragments from your own armor | Hook on death + plate attr cousin |
| **Wetwired** | Ion / Lightning taken −; `Zapped` lasts longer when it does land | Radio pack hates this guy, and also loves him |
| **Hot-blooded** | Start the fight `Hot` (body). Energy out up, kinetic out down, guns overheat | Status you already have, pinned on |
| **Cold-blooded** | Start `Cold`. Inverse. Metal embrittle still applies to *you* | Do not stack with Ice cold as a third heat row |
| **Glass core** | Energy / ability budget up. Core socket armor down one rung | Walkers and borgs. Humans skip |
| **Cook-off** | Fuel / Battery sockets ignite easier. Pact’s ugly cousin without the cooldown gift | Hook-ish: heat threshold lower on those sockets |
| **Spare magazine** | Reload and ammo pickup + | Boring on purpose |
| **Heavy boots** | Knockback / ragdoll time down. Slide distance down | ShearThick cousin as a pawn number |
| **Redline** | Move and recharge scale up as HP crosses Medium → Low | Hook on `BandChanged`. Elites. Not Ryko’s scream |
| **Last word** | On `Died`, short Ion splash (emp) + dump remaining heat | Send-off for energy users. Same Died hook slot — pick one |
| **Blood tax** | Your stim / pack heals also apply a small `Bleeding` or heat to you | Hook on heal write. Makes Über greedy |
| **Coward’s plate** | First hit each fight Nulls if it would have been Bounce/Damage at the edge | **Illegal.** That is a Resolve fork. Do not. |
| **One more** | After crit / Nuh uh, one pack pulse is free | Hook. Requires Nuh uh or a real crit band |
| **Ping magnet** | Your pings last longer; utility drone / autofense priority treats your marks as top | Perception only |
| **Oiled** | Start `Oiled`. Slip + flammable. Funny on purpose | Status you already have |
| **Inspire** | Nearby friendlies get a short Encourage from morale table when you get a kill | Morale, not Health. AI folder later |
| **Jammed** | You are a walking jam cylinder. Radio packs near you fail. Your own radio fails | Status zone. Open-sky maps |
| **Bounce back** | See locked list | Jump / bounce kits |
| **Fused** | Start `ExplosiveCoated`. Overwhelming blunt, Burning, or Shocked can cook you | Send-off’s living cousin. Funny, rare |
| **Redundant** | After a limb’s first injury, further hits glance unless center (higher AP through the hole) | Ryko rule as a perk for elite machines. Humans skip |
| **Unfeeling** | `hasFeelings = false` | Drones default this. Shock troops can take it |
| **Mean** | Your cheap taunt / kill applies `Enraged` or Terrified to nearby AI by type | Morale table. Player taunt button still owns the verb |

| **Short fuse** | Pack / hip recharge +, capacity − | Nervous kit. Pact’s small cousin |
| **Long fuse** | Capacity +, recharge − | Opposite. Deny with Short fuse |
| **Magnet hands** | Pickup / corpse / plate-drop radius + | Cannibal lite. Ryko already magnets mass |
| **Bloodhound** | Last pawn you damaged outlined 0.5 s through walls | Seventh Sense. Perception only |
| **Tunnel** | After you take a hit, accuracy up vs that instigator, FOV cut | Feelings + aim. Conscripts take this and get ugly |
| **Officer** | Nearby friendlies ignore cheap taunts. Your death Terrifies them | Morale. Delete the officer |
| **Conscript** | Cheap taunt / loud noise scatters you. Move + when running away | The other half of Officer |
| **Spare eye** | One-eye Blinded is FOV cut only, never full blind until both gone | Eye socket already works this way; perk makes the first eye cheap |
| **Deaf** | No `Deafened`. Hear radius − | Flashbangs become lights |
| **Spark trail** | Dash / wallrun write tiny Thermal decals and noise | Tell. Heat bite likes this |
| **Clean** | Incoming DoT duration − | Not a resist channel cheat; clocks only |
| **Anchor** | Knockback / grapple-yank on you − | Deny Bounce back. Heavy boots cousin |
| **Kite** | Knockback on you +. You keep air control | Deny Anchor. Bounce back stacks |
| **Deep pockets** | +1 throwable charge | Boring |
| **Filter** | Toxic heal-cut and flesh DoT − | Spare kidney. Borgs skip |
| **Open face** | Head / visor armor −1 rung, ADS and lock + | Ladder honest: you stamped a worse plate |
| **Mirror visor** | Head segment gets `Mirror` | Laser jokes bounce. Kinetic still loves your forehead |
| **Sleeper** | While still, `LowVis`. Breaks on fire / dash / pack pulse | Cloak pack still owns real invis |
| **Restless** | Hip / pack regen only while moving | Deny Sleeper. Hover pack loves this |
| **Iron lung** | `Stimmed` duration +. Bleed / heat while Stimmed + | Stim pack’s mean twin |
| **Thin blood** | Bleed slower. Stim / siphon weaker | Deny Iron lung |
| **Adrenal** | Crossing Medium applies a short `Stimmed` | Hook on band. Second wind’s cousin |
| **Pacemaker** | Crossing Low applies `Amped` | Machines / FleshBorged only |
| **Buddy** | Damage and flinch resist + if an ally is in 8 m | Deny Isolated |
| **Isolated** | Damage + if no ally in 12 m | Deny Buddy |
| **Flak** | Autofense / lock priority vs airborne + | Perception / turret def |
| **Skyblind** | Opposite. Worse vs air, better vs ground | Deny Flak |
| **Shoulder** | Sprint collision is a small Blunt packet | Source already hits. This just authors the packet |
| **Head down** | Crouch move +. Standing profile unchanged | Stillness cousin |
| **Stillness** | While crouch-still: lock +, recoil −, intel ping + | Entrenched’s small brother |
| **Revenge** | Last instigator outlined until you damage them or they die | Bloodhound but they started it |
| **Counterpunch** | After taking melee, your next melee windup − | Hook on Damaged if source is Fist / Blade |
| **Scavenger** | Ammo / battery from boxes and corpses + | Spare magazine’s world version |
| **Cannibal** | Corpse siphon radius + | MP echo of Ryko mass. Tiny numbers |
| **Trophy** | Melee kill strips a hanging plate from the corpse into your inventory | Hook on kill. Engineer / juggernaut like this |
| **Dead switch** | Send-off only if executed or Core popped | Same Died slot as Send-off / Last word |
| **Spare fuse** | Send-off beep is longer. Teammates can run | Modifier on the hook, not a second hook |
| **Brittle pride** | Thick armor numbers, but Ceramic shatter threshold down | Must also have Thick. Ugly tank |
| **Soaked** | Start `Wet`. Fire almost off. Cannot `Charged`. Shock hurts more | Bubble aura dies. Thermal hip shrugs |
| **Static** | After dash / vent, brief `Charged`. Contact tick on bump | Poor man’s lightning-bubble. Wet denies |
| **Conductor** | Outgoing Ion / Lightning chains one extra hop | Tesla as a stamp. Walkers and Wetwired care |
| **Grounded** | Incoming chain lightning stops on you. You eat the extra as heat | Deny Conductor. The sink |
| **Coolant bath** | Your Coolant leak applies `Wet` in 3 m | Ally thermal gift, your shock problem |
| **Flashbulb** | On `Died`, Blinded + Deafened splash | Same Died slot as Send-off / Last word / Flashbulb — pick one |
| **Possum** | On first crit band, forced ragdoll + `LowVis` for 1.5 s | Nuh uh’s coward twin. Bounce back launches you out of it |
| **Zealot** | Ally death in 12 m applies `Enraged` to you | Morale. Officer dying is the feature |
| **Mute** | Cannot taunt. Immune to cheap taunt scatter | Mean’s inverse. Elites |
| **Ugly** | First sight of you Terrifies conscripts in 6 m once | Morale tell. Then they just shoot |
| **Appraiser** | Looking at a pawn shows plate level / heat / wet | UI only. Engineer and bubble-bros |
| **Kickback** | Kinetic recoil adds wish-velocity | Property on the shooter. Eternal shotgun as a stamp |
| **Live round** | Held throwable cooks after N seconds | Funny. Send-off while alive |
| **Sealed** | Acid and Wet apply slower | Not immune. Borg seals, humans sweat |
| **Rustbelt** | Acid and Wet apply faster. Heat dump + | Deny Sealed. Old machine |
| **High rider** | Hover / air drain −. Ground wish-speed − | Hover pack identity |
| **Dirt cheap** | Fall / slam self-damage −. Slam packet − | Soft lander. Deny Shoulder / Afterimage slam pay |
| **Spare heart** | Bleed clock + one extra band before flesh death | Humans only. Nuh uh is the full version |
| **No cockpit** | No occupant socket. Door-rip / pilot-kill hooks miss | Walkers / drones. Death’s Kiss shrugs |
| **Loaner** | Ally in 6 m can spend one of your pack charges | Hook on their TryPulse. Supply’s living cousin |
| **Stingy** | Allies cannot use your kit. Your pickup radius − | Deny Loaner / Magnet hands |
| **Green** | Weld / plate-repair rate + | Engineer pack. Trophy strip likes this |
| **Sweaty** | High heat slowly applies `Wet` to you | Self-anti-charge. Thermal dump as rain |
| **Chain smoker** | Start `Hot` + `Oiled` | Firework. Pact’s roommate |
| **Famous** | Always treated as Marked | Intel maps. Bosses default this |
| **Left heavy** | Melee windup −. Gun accuracy − after a melee until you wait | Gloves off. Brawler stack |
| **Pretty** | First cheap taunt against you fails, then you `Enraged` | Valkarie-shaped. Once per fight |

Illegal on purpose: anything that changes AP ladder math, invents invuln frames, or adds a third hip.

---

## Where a perk is allowed

| Tag | Meaning |
|---|---|
| **S** | Species inherent. Head only. Not a shop pick. |
| **B** | Baked on a **piece**. Torso cores can ship **more than one**. Legs/helmet can bake small ones (Heavy boots, Mirror visor). |
| **P** | Player **pick**. One extra on the card. |
| **T** | Training / HUD. Visor firmware. |
| **AI** | Entity stamp. Not on the operator pick list unless a mutator. |
| **X** | Illegal |

A bulwark chest is legal as `B+B`: **Deep pockets** + **Reinforced plating** (Thick’s plate half without the full −20% speed, or Thick itself). Martyrdom (**Send-off**) is a **B** minor. It lives on a chest, not a hat.

Same field twice: second copy half. UI hides a P that duplicates a B on the equipped core.

### Species (S) — not perks

| Plan | Inherent |
|---|---|
| Human | EMS-proof-ish (ion-slow / EMS fields shorter) |
| Cat | Smaller, burst +, stamina drain +, flash −, night eye, no motion blur, same meat EMS |
| Machine | Sauce −, stamina **is** energy, brown-out not gasp, no meat EMS, Zap/Amped legal |

### Classification

| Perk | Tags | Notes |
|---|---|---|
| Send-off / Martyrdom | B, P | Chest bomb. Helmet no |
| Spare fuse / Dead switch / Last word / Flashbulb / Loose plates | B, P | Same Died family. One Died hook per pawn |
| Nuh uh / Spare heart / Possum / One more | P, AI | Meat crit. Not a chest fashion |
| Pact / Cook-off / Chain smoker / Fused / Live round | P, AI | Joke bombs. Rare B on a marked “suicide” core |
| Thick / Reinforced plating / Brittle pride | B, P | Bulwark language |
| Too light / Scout quiet crumb | B, P | Scout shell |
| Ice cold / Heatsink / Heat-dump / Sealed / Rustbelt / Sweaty | B, P | Heat-dump torso = plate − plus overheat off |
| Overclocked / Short fuse / Long fuse | P | Hot-rod. Rare B |
| Deep pockets / Spare magazine / Scavenger / Magnet hands | B, P | Bulwark + Deep pockets is the example |
| Heavy boots / Anchor / Dirt cheap | B (legs), P | Legs bake these |
| Mirror visor / Open face / Spare eye / Deaf | B (head/helmet), P | Helmet minors. Still not core |
| Soft step / Clatter / Spark trail | B, P | |
| Flak / Skyblind / Filter / Clean / Wetwired / Grounded / Conductor | B, P | |
| Brawler / Left heavy / Counterpunch / Trophy / Cannibal | P | Hands. Not a chest |
| Bounce back / Kite | P | |
| High Alert / Tracker / Bloodhound / Revenge / Appraiser / Birdseye / Ghosted / Sitrep / Blank signature / Ping magnet | T, P | HUD/training first. Can be P if we drop T row |
| Operator motivator / Infusion / Restock | P | Needs stim economy |
| Redline / Adrenal / Pacemaker / Iron lung / Thin blood / Blood tax | P, AI | |
| Unfeeling / Redundant / No cockpit / Glass core | S-adj, AI, B on machine pieces | |
| Officer / Conscript / Inspire / Mean / Mute / Ugly / Pretty / Zealot / Famous / Marked | AI | Morale |
| Coward’s plate | X | |
| Hot-blooded / Cold-blooded / Oiled / Soaked / Static | B, P | Status pins |
| Buddy / Isolated / Loaner / Stingy / Green / High rider | P | |
| Kickback / Shoulder / Head down / Stillness / Sleeper / Restless | B, P | |

---

## Pairing notes

- Overclocked + Pact = you are a firework. Legal. Thermal hip on that pawn is almost cheating; allow it anyway.
- Thick armor + Too light = deny on the sheet.
- Ice cold + Hot-blooded = deny (Hot vs Cold body already conflicts).
- Nuh uh + Send-off = beep starts when crit ends, not at first 0. Otherwise the body explodes while still punching.
- Brawler + kill-to-heal campaign Ryko = she already does this. Do not stack a perk on her for it.
- Bounce back + Heavy boots = deny. Bounce back + Anchor = deny. Bounce back + Kite = intended.
- Nuh uh + Bounce back = crit launch instead of crit flop. Legal and ugly.
- Short fuse + Long fuse = deny. Sleeper + Restless = deny. Buddy + Isolated = deny. Iron lung + Thin blood = deny. Flak + Skyblind = deny.
- Kits stay the verbs. Perks only change how expensive or loud those verbs are.

---

## Player perks (MP operators)

Two lanes. You pick **one Fit** and **one Habit**. Entity perks above can also stamp an AI or a freak; this list is the menu humans see.

Campaign Ryko does not shop here. Velocity-punch, kill-to-heal, glance-after-break are already her sheet.

### Fits — always on, boring on purpose

Numbers only. If it needs a sentence that starts with “when,” it is a Habit.

| Fit | Touch |
|---|---|
| **Spare plate** | + plate HP. No extra attribute slot (that is Thick armor, an entity perk). |
| **Deep hip** | + JumpKit / battery / thermal capacity. Regen unchanged. |
| **Quick hands** | Reload and pack swap faster. |
| **Clear visor** | `Blinded` / `Deafened` / muzzle wash shorter. Sensor pack stacks. |
| **Long throw** | Nade / hook range +. |
| **Padded** | Fall, slam, and self-recoil down. Slide a bit worse. |
| **Quiet kit** | Hip and pack noise down. Soft step is the full-body version. |
| **Hot spares** | Energy weapons start colder; first heat write each fight is halved. |
| **Calibrated** | Smart-lock a little faster. Sensor still wins if both are on. |
| **Spare fuse box** | +1 throwable. Deep pockets as a Fit name. |
| **Magnet** | Pickup radius +. |
| **Filter mask** | Toxic / flash chemical duration −. |
| **Steady** | Recoil and self-impulse −. Hipfire a bit worse. |
| **Long breath** | Slide / crouch / hover drain −. |
| **High vis** | You see further in dark / smoke. Intel sees you further. |

### Habits — a verb pays you differently

Still no new buttons. The button is already on the hip, pack, gun, or melee.

| Habit | When | Payoff |
|---|---|---|
| **Afterimage** | After a jumpkit dash | Melee / body hurtbox swells for a beat. Slam extras. Movement doc already wanted this on Ryko; MP buys it. |
| **Heat bite** | Melee while your heat is high | Fist packet gains Thermal. Self heat dumps a slice. vladddoi recycler as a habit, not a mech-only toy. |
| **Bloodthirst** | You deal damage | Tiny pack / hip recharge from the packet magnitude. |
| **Second wind** | You cross Medium → Low | One short speed burst + heal tick. Redline is the AI version that keeps scaling. |
| **Entrenched** | Not moving | Reload, bubble regen, and thermal dump faster. You ping louder on intel. |
| **Killer calm** | You get a kill | Flash / flinch / recoil reset. Visor clears. |
| **Feed the bubble** | Your own plasma / lightning hits your bubble | Extra HP feed / Charged budget. Makes the aura joke reliable. Without this, it still works, just stingier. |
| **Slide rule** | High-speed slide | Extras ragdoll easier. You keep more speed off ramps. |
| **Marked man** | You ADS | Sensor-style outline in 2 m. Active Sensor still owns the 20 m pulse. |
| **Last magazine** | Mag hits empty | Next reload is faster and that mag does a little more kinetic. Then it goes away. |
| **Courier** | You use Supply / Stim on an ally | You get a smaller copy of the buff. |
| **Grapple yank** | Grapple attaches to a pawn | Pull them if they are lighter; pull you if they are heavier. Default grapple is mostly surfaces; this habit makes it a person-thief. |
| **Hot landing** | Hover pack touchdown | Small Thermal splash under you. |
| **No witnesses** | Hollo decoy detonates | Short `LowVis` on you. Cloak pack still owns real invis. |
| **Recoil step** | You fire a kinetic gun | A slice of recoil becomes wish-velocity. Battery hip users skate. |
| **Revenge mark** | Someone damages you | Outline them until you hit them back. |
| **First blood** | First packet you land on a pawn | Tiny hip refill. Once per target. |
| **Air tax** | You are off the floor | Incoming flinch −, your kinetic −. Energy unchanged. |
| **Crouch load** | You reload while crouching | Reload faster, louder ping. |
| **Buddy system** | Ally in 8 m | Your stim / supply range +. Isolated players waste this. |
| **Lone gun** | No ally in 12 m | Hip regen +. Sensor pulse a little further. |
| **Executioner** | Glory / melee kill on a humanoid | Faster windup, bigger heal tick. Not Ryko’s full siphon. |
| **Reload dash** | Reload finishes | Short speed burst. Empty-mag panic button. |
| **Painkiller** | You cross a health band | Flash / stun resist for a second. Visor stays up. |
| **Shoulder check** | Sprint hits a pawn | Small Blunt packet. Bounce back turns it into a joke. |
| **Sleeper cell** | You have been still 1.5 s | `LowVis` until you fire or dash. |
| **Restless hands** | You are moving | Hip regen +. Standing still starves the bar. |
| **Counter** | You ate a melee | Next melee is fast and knocks more. |
| **Trophy strip** | Melee kill | Yank a shattered plate as a tiny HP/armor crumb. |
| **Vent bite** | Thermal hip hold ends | Short speed burst. You just opened the radiator. |
| **Empty threat** | Mag dry-fires | Cheap taunt pulse. Mute wastes this. |
| **Swap cool** | Holster gun / pull melee | Dump a slice of weapon heat. Heat bite setup. |
| **Plate break** | You shatter someone else’s plate | Tiny Stim tick. Ceramic farmers. |
| **Head tax** | Head / visor hit you own | Hip refill. Open face users live here. |
| **Zip leftover** | You die with a zipline up | Line stays N seconds. Courier / grapple maps. |
| **Pack mule** | You are hit in the pack actor | Pack HP +, you flinch −. Tight straps. |
| **Hot swap** | Weapon swap | First shot of the new gun does not write heat. |
| **Wall gift** | Wallrun ends in a jump | Extra up impulse. Bounce back stacks. |
| **Wet work** | You are `Wet` | Shock taken +, fire taken −, bubble will not Charge. Play the soaked kit on purpose. |
| **Dry fire** | You are `Hot` and not Wet | Energy packet +, sprint noise +. |
| **Borrowed time** | Nuh uh / crit is live | Pack cooldown halved. One more’s cousin. |
| **Mercy spark** | Stim an ally who was on fire | Also clear Burning on them. Courier’s medic read. |

Deny on the sheet: two Habits that both key off the same event if they would double-pay (Bloodthirst + Killer calm is fine; Bloodthirst + Second wind-on-damage is not a thing). Feed the bubble without a plasma pack is a wasted row — hide it in UI.

### What player perks are not

- Not a third pack.
- Not Nuh uh / Send-off / Pact unless you want operators to be funny bombs. Those stay entity stamps for AI and named freaks. If an MP lobby wants Send-off as a wildcard, it is a mutator, not a Fit.
- Not AP cheats. Thick / Too light already swing plate HP. Ladder stays honest.

---

## Stolen (CoD / Helldivers 2)

HD2 wasted the two “please let me play the game” rows on booster slots. We do not.

**Default, not a perk**

- Spawn / resupply with hip, nades, and stims **full**. That is match rules, not Fully packed.
- Limb injury and after-plate incoming are already the armor ladder + feelings. Vitality-as-10%-less-damage is not a Fit. Thick / Spare plate exist if you want to *buy* tank.

If someone still wants a Hellpod-shaped row, it is greed, not baseline:

- **Overstock** (optional Fit): stim capacity 4 → **6**. Not “you finally get ammo.” You already had ammo.

Squad-wide HD2 clocks (Confusion, Extraction, reinforcement budget) are lobby mutators. Stopping Power stays illegal. Ghost does not beat an **active** Sensor pulse.

### More Fits

| Ours | Stolen from | Touch |
|---|---|---|
| **Overstock** | Hellpod opt, if it must exist | Stim max 6 instead of 4. Spawn is already full. |
| **Long wind** | Stamina Enhancement | Hip regen *and* duration, not a sad +10%. Deep hip is capacity only. |
| **All-terrain** | Muscle Enhancement | Mud, slope, sand, snow, `Oiled` slip: speed penalty *gone*, not trimmed. |
| **Flak** | Flak Jacket / EOD | Explosive + incendiary taken −. Firebomb jokes hurt you less. |
| **Gung-ho** | Gung-Ho | Fire and reload during sprint / slide. |
| **Dexterous** | Dexterity | Air and slide gun wobble −. Fall impact −. |
| **Grenadier** | Grenadier | +1 throwable. Long throw is range; this is count. Spare fuse box already exists — alias it. |
| **Extinguisher** | Integrated Extinguishers | `Burning` drops faster on you and a teammate you use. |
| **Blank signature** | Cold-Blooded (CoD targeting, not our temperature perk) | Smart-lock and autofense treat you late. No thermal outline. Does not start you `Cold`. |
| **Ghosted** | Ghost | Moving = Sensor **passive** / radar skip. Standing still pings. Active pulse still sees you. |
| **Sitrep** | SitRep / Bomb Squad | Mines, satchels, hollo bombs, cloak drones outline at 8 m without Sensor. |
| **Hardline** | Hardline | Pack cooldowns − a real notch. Match-cap orbitals stay match-cap. |

### More Habits

| Ours | Stolen from | When | Payoff |
|---|---|---|---|
| **Operator motivator** | (ours; HD2 stim-on-kill that should have existed) | You own a kill | One stim charge back (cap still 4, or 6 with Overstock). No heal until you pop it. |
| **Scavenger** | Scavenger | You own a kill | Ammo + one nade crumb. Ryko’s mass siphon stays campaign-only. |
| **Restock** | Restock | Clock | Lethal / tactical recharge. Stim slower than nades. |
| **High alert** | High Alert | An enemy ADS you | Visor tick + direction. Blank signature does not trigger it. |
| **Tracker** | Tracker | You look at ground they used | Foot / heat trail. Not walls. |
| **Resolute** | Resolute | You take a hit | Short speed burst. Second wind is the band version. |
| **Quick fix** | Quick Fix | You get a kill | Small heal. Do not stack with Brawler. |
| **Dead sprint** | Dead Sprint | Hip hits 0 and you still hold sprint | Keep speed, spend HP / heat. Pact’s cardio cousin. |
| **Infusion** | Experimental Infusion | You Stim | Extra speed + resist while `Stimmed`, tunnel visor. |
| **Motivational** | Motivational Shocks | Something applies slow | Ignore enemy slow tags. Broken legs and EMS still count. |
| **Firepod** | Firebomb Hellpods | You drop in / resupply lands | Incendiary splash. FF legal. |
| **Stun pod** | Stun Pods | Same drop | Ion / `Zapped` field. Deny Firepod on the same sheet. |
| **Armed crate** | Armed Resupply Pods | Supply pack lands | Short autofense on the box. |
| **Sample luck** | Sample Scanner / Extractor | You loot | Chance of double crumb. Score / campaign pickup. |
| **Tempered** | Tempered | A beat after a hit | Plate / bubble HP starts back sooner. |
| **Birdseye** | UAV Recon / Birdseye | Intel pulse | Radar / Sensor range +. Pulse still needs the pack. |
| **Extraction** | Expert Extraction Pilot | Match is extract | Egress timer −. Hide off those maps. |
| **Confusion** | Localization Confusion | You are alive | Unscripted reinforcement clock +10%. Scripted scenes ignore it. |
| **Surplus tube** | Surplus EAT Allocation | Match start | +2 micro-missile charges once. |

---

## Do not build yet

Same board focus: plate-soak, HurtBox, JumpKit compose.
When AI starts, first grunt is a sheet with JumpKit dash + maybe Soft step. Not a perk tree UI.
