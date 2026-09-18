# Melee

Left hand is a **toggle**. That toggle changes what **Interact** and **Melee** do. Right hand keeps the gun. Extra-limbs pack does not steal this hand.

Parry spin is **microjets in the pawn’s hands**. The tool is dumb. You aim the face.

Same packets and plates as guns.

---

Weapon-out Interact is never “pick up.” It is charge / recall / detonate / sword swirl.

---

## Fist mode

### Interact

Tap once: **grab**. Tap again on a held *object*: **set down**.

- Mass ≤ your strength: you pick it up. You carry it.
- Mass ≥ **2× you**, or it is level geometry: you **grip**. You do not lift it. If it moves, you move.

### Melee / interact-while-busy

| State | What happens |
|---|---|
| Holding an object | **Throw.** Impact packet on the thrown thing. Weaker actors with no stability **ragdoll**. Anyone the flying mass hits ragdolls if the hit exceeds *their* threshold. |
| Gripping a surface | **Launch back.** Space in this grab state is the same kick-off (back, off the wall). |
| Empty, enemy in range | **Punch.** Knockback. Stun if your arms have the force / a punch perk. |

### Grip break (you are held, or you hold a person)

Held actors still take packets. They are a **meat shield** until the grip dies.

Break out with: movement ability, repeated punches, grenade, or a jet burst. Struggle is **additive** and sits on a short cooldown per try.

If their grip strength is **≥ 4× yours**, you do not break on struggle alone. You need the additive burst (jets + nade + ability) to pop it.

### Strength is a sheet attribute

Grab / throw / punch read **Mass** and **Strength**. **Agility** is swing prep + knockdown resist. **Speed** is how fast the tool moves. Fields: `10-mp-cosmetic.md`.

Species Strength: **Machine → Human → Cat-upper.** Cat legs = human. Ryko is Machine plus super-chassis on that same field.

Arms multiply upper. Legs multiply legs. Kick-off uses legs. Throw and punch use upper.

### Campaign named grabs

| Target | Grab | Throw / break |
|---|---|---|
| **Enduring** | He can grab **you**. Same verbs, turned around. Tables flip. | He throws Ryko if the latch sticks. |
| **Valkarie** | Latch works. | Her jets are enough acceleration to **break the hold**. You do not keep her. |
| **Arsonist** | Latch works for a beat. | Flames cook and **explode you off**. |
| **Sniper** | Works. Mini-boss, not a prop, but the grab sticks. | Throw legal. |
| **Redline** | Works. Same mini-boss rule. | Throw legal. |
| **Redline’s bike** | You hold the bike or you **slip off**. You do not get a clean yeet of the whole vehicle unless the sheet says the bike is already wrecked / unseated. | |

Named breaks are sheet flags on those pawns (jet burst, flame burst, 4× grip), not special Resolve.

---

## Weapon mode

**Melee** = swing (press or press-release per def).

**Interact** = that tool’s charge / recall / detonate / special.

### Anime sword

**Interact hold — swirl**

Auto-parry in the spin volume, **including hitscan**. Also slices enemies in that volume. Eats **massive stamina and heat** while held. **10 s CD** when you drop it.

**Melee tap — cut**

For **0.1 s**, slice every enemy in range and parry every projectile in range. **0.7 s CD**.

Jets spin the steel. The steel does not think.

### Other Interact specials

| Tool | Interact |
|---|---|
| Sledge | Windup from carry (this key owns the windup) |
| Buckler | Ram / brace |
| Chainsword | Rev |
| Dagger | Recall / detonate stuck charge |
| Monowire | Tauten / reel |
| Spear | Recall |
| Axe | Heavy hang |

---

## Weapons

### Anime sword

CNT + tungsten + classified micromachined alloys. High-AP kinetic slash. Designed to cut plates that other melee shrugs.

See Weapon mode numbers above. Ryko campaign default left. MP can buy a cousin.

### Explosive-head sledge

Heavy hammer. No tap-swing from idle.

Hold from carry: it **winds up**. Release at any height. Higher = more packet + more self-tax.

On connect: crushing kinetic + Explosive. Bonus liquefaction vs meat. Hit the floor: shockwave ring (Explosive + knockdown, short).

While held up: move −, aim sway +, stamina / thrust drain +. Enduring campaigns an XXL sheet of the same verb.

### Directional buckler

Armor plate on forearm + upper arm. That limb’s plate HP goes up. Punch packet goes up.

Close-range **ram**: pick a direction, shockwave on contact. Brief parry window after the ram (jets + plate, not a forcefield).

Run speed − while equipped.

### Chainsword

Toothed loop, continuous cut. Tick kinetic while the teeth are in a volume.

No parry. You do not face rounds with this. You walk it through a torso.

### Elemental dagger (explosive / lightning / gas)

Arm-mounted cannon launches it. Stick = fuse.

- Explosive: splash on stick.
- Lightning: Ion / Charged on the stuck actor (same Charged rules as the lightning gun).
- Gas: Toxic / cloud on stick.

Synergy: reel back with grapple. **+1 grapple charge** on a successful stick-and-reel.

Throwable. Leaves the left hand empty until it returns or you swap.

### Monowire

Wire from the wrist. Long melee reach, thin hit. High AP on a taut line, low on a slack whip. Cyberpunk cousin as a **verb**, not a license.

### Arm blade

Cyberware **replaces the left arm**. Rapier extends from the palm center (not a mantis-scythe).

Same toggle conceptually: retracted = that arm is a hand with a hole; extended = rapier. Grab is weaker or gone on that side.

### Axe

Classic. Medium windup, high limb-break vs plates, no splash.

### Vampiric returning spear

Throw or thrust. Returns.

Heal: a cut of HP you **took from that enemy** comes back to you (siphon of damage landed, not a second health pool). Empty target = no drink.

---

## Hands without a tool

Grab range is melee. Throw uses the target’s mass vs your sauce. Microjets can add a shove.

This is how you yeet a grunt without a sledge.

---

## Sheet notes

- Sledge hold tax writes stamina / thrust and a sway crumb. It does not invent a new meter.
- Buckler is a plate on Arm segments + a short Status after ram (`Parrying` or reuse existing block window).
- Dagger elements are packet channels already on the damage sheet.
- Spear siphon is a heal write from damage dealt, same family as kill-to-heal crumbs.
- Chainsword is a volume tick, not a saw-minigame.
- Arm blade is a Legs/Arms piece tag (`FleshBorged` optional on that arm) plus the rapier def.

Avoid-list stays in `design notes/README.md`.
