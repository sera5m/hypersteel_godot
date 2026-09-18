# MP operator cosmetics

Campaign named freaks do not shop here. This is operators only.

## Wetware baseline (meat)

Every MP meat agent is already full of **standard cybernetics and chemicals**. The species table is the number **with that stack on**.

Strip it (no implant, no stim cycle) and the same body sits **~20% under** those rows — Speed, Strength, Stamina, Agility, Heal. Not a perk. Not a loadout slot. Loading-screen text: “your agent is full of standard cybernetics.” Then never mention it again.

Cat combat implant: creatine-to-hell plus the usual suite. Human: the human cousin of the same suite. **Both meat species.** Different recipe, same job. Unaugmented Redline in campaign is a different sheet than an MP cat.

Machines do not get a creatine pump. Their Strength/Speed rows *are* the chassis.

MP operators are some flavor of special forces / military. No org chart. Players invent squads. Ops, if they exist, are Helldivers-shaped jobs with another team trying to stop you — not a campaign chapter.

---

## Map (what writes what)

Fits as a menu row are dead. They were +3% or a class lock.

| Piece | Job | Examples |
|---|---|---|
| **Head** | Species attribute. Always on. Not a pick. | Cat: flash −, night eye, no motion blur, burst +, gas +. Human / cat meat: EMS-proof (ion slow / stun fields shorter). Machine: never gases, slower, heat soaks into chassis. |
| **Segments** | Stats + attach points. | Heavy legs: speed −, plate +. Helmet can have plate. |
| **Armor core** (chest) | **One or more baked minors.** Swap chest, swap those rows. | Bulwark: Deep pockets + Reinforced plating. Martyrdom chest: Send-off. Heat-dump: plate −, overheat off. |
| **Picked perk** | **One you select** on top. | Tagged P, and not already baked on this chest. |
| **Training / HUD** | **One sense.** Visor firmware / school. | High Alert, Tracker. |
| **Hip / pack** | Buttons. | Jumpkit, bubble. |

Baked minor ≠ picked perk. A robot core does not eat your pick so you can “have heatsink.” Heatsink is why you bought that core. You still pick Sitrep.

Helmet cannot be the core and cannot bake Died hooks. **Martyrdom is a chest minor.** Hat-martyrdom stays illegal.

Species attribute and core minor can both touch heat (cat tail-dump + machine heatsink). Stack rule: same field twice, second copy half.

### Species inherents

Body-base crumbs still stack on top. Machines have no body-base row.
Percents vs human meat baseline. Sheet multipliers / Status durations, not Resolve forks.

#### Cat

- Shock / ion **62%** resist
- Flashbang **40%** resist (lids slam)
- Inherent night vision; motion blur off
- Knockdown / ragdoll **70%** resist (tail)
- Recoil **−40%**
- Heat tolerance **+15%**
- Move **+33%** vs human
- Hand speed **+30%** (melee + use-item)
- HP **−10%** (smaller)
- Stamina **−40%**
- Quiet-sound hear radius **+40%**
- Deafen taken **+15%** (loud / explosives)
- Explosive taken **+5%**
- **Bleeds.** Blown limbs bleed out like humans

Same rows for all three. Human is the 100% column except your set numbers.

| | Cat | Human | Machine |
|---|---|---|---|
| Shock / ion resist | 62% | **70%** | **−40%** |
| Flashbang resist | 40% (lids) | 0% | −15% (cameras bloom) |
| Night vision | Inherent | None | None (IR is a helmet piece) |
| Motion blur | Off | On | Off (sampled) |
| Knockdown resist | 70% (tail) | 0% | 25% (mass + gyros) |
| Recoil | −40% | 0% | −20% (mounts) |
| Heat tolerance | +15% | 0% | +10% chassis; heat-dump chest replaces this row |
| Move vs human (Speed) | +33% | 0% | −12% |
| Agility (was hand speed) | +30% | 0% | +10% (servos) |
| **Use time** | **−25%** (six-finger anim) | 0% | −10% (servos) |
| **Grip** | **+20%** (sixth finger / extra thumb) | 0% | +25% (clamp) |
| **Strength (upper)** | **−25%** | **0%** | **+40%** (servos) |
| **Strength (legs)** | **0%** (same as human) | **0%** | **+20%** |
| Ability CD | 0% | 0% | **−8%** |
| HP | −10% | 0% | **+35%** |
| Stamina / energy | −40% stamina | 0% stamina; heat separate | **Merged with jumpkit** |
| Quiet hear radius | +40% | 0% | +10% (mics) |
| Deafen taken | +15% | 0% | **−50%** |
| Blind taken | 0% after flash resist | 0% | **+18%** |
| Explosive taken | +5% | 0% | −10% |
| Bleed from blown limb | Yes | Yes | No. Fuel / Power / Coolant leak |
| Gender row | Yes | Yes | **None** |
| Bars | Two | Two | **One energy.** Brown-out. Energy guns dump in. Carry adds battery stock. Bar leaks. Generator chest refills. No generator = leak only |

### Body-base crumbs (meat only)

Same UI filter as race. Not a perk. Machines skip this table.

| Plan | Masc | Andro / enby | Fem |
|---|---|---|---|
| Cat | +4% HP, −2% spd, −2% agi | +5% faster ability CD, +5% agi, −1% spd | −5% HP, +3% spd, +3% faster ability CD, +2% heat buildup |
| Human | +8% HP, −6% agi, +4% spd, +5% worse heat buildup | +4% agi, +2% spd, +8% faster ability CD | +10% agi, +2% spd, **−5% HP** |

`agi` and `spd` do not alias.

## Character attributes (sheet fields)

Pieces, species, body-base, and perks only multiply these.

| Field | Does |
|---|---|
| **Mass** | Accel time to wish-speed. Grab / punch / throw results. Melee influence. High mass + low Strength = furniture. |
| **Agility** | Air control. Item use / APM. Weapon handling. Turn rate. Swing prep. Knockdown resist (with Strength). **This is hand speed.** Optional later split: movement-agi vs dexterity. |
| **Speed** | Max velocity. Actions per second. How fast the weapon travels through the cut. |
| **Stamina** | Jumpkit and/or sprint. Same job. Machines merge with energy. |
| **HP** | One magnitude, auto-split across torso + limbs. |
| **Plate / armor** | What a packet does to that HP. |
| **Heal rate** | Subtypes: adrenaline (temp HP), permanent occupant HP, plate repair. Meat keeps occupant vs plate separate. Machine smears them but still two writes. |
| **Thermal tolerance** | Hot and cold. Past the band: uncomfortable, then Status. |
| **Recoil** | Kick. Modified by Strength and Agility. |
| **Strength** | Base melee damage on every melee. Grip. Grapple break / door rip. Part of knockdown resist. Tiny speed help if Mass is already huge. |
| **Ability CD** | Wait after a pack / hip / item use. |
| **Use time** | Length of the activate animation. Agility shortens this. CD is the wait after. |

Cat +30% Agility is the old hand-speed row. Six fingers also write **Grip +20%** and **Use time −25%** (different grab/use anim). Strength-upper stays −25%, so they cling harder than they lift. Speed +33% is the third fast row. Strength rank: Machine > Human > Cat-upper; Cat legs = Human.

### Armor core examples

| Core | Stats | Baked minor |
|---|---|---|
| Heat-dump torso | Torso plate − | Overheat off / heat dumps. Thin chest. The robot “I run hot guns and live” chest |
| Heavy robot core | Torso plate + | Heatsink crumb. Slow |
| Sealed meat plate | Normal | Wet/Acid slower |
| Scout shell | Plate −, speed + | Quiet crumb |

---

## Paint vs skin vs mesh

| Layer | What it is | Who can wear it |
|---|---|---|
| **Paint** | Color groups on existing materials. Library swatch, raw RGB, or a custom camo triad | Any piece that exposes those groups |
| **Pattern** | Library stamp sampled into paint groups. **Metalheart chrome glyphs + cyber-occult marks are camo**, not merch. Cable-jungle breakup the way tiger stripe does leaves. Hex / urban still exist. | Same |
| **Skin** | Either a new pattern source **or** a new mesh in a slot | Only if the slot and **body plan** match |

A new cat-head is a **skin** on Head / plan `Cat`, not a paint.

Four segments each write crumbs and attach points. Helmet can have plate. Paint is a loadout layer (primary / secondary / accent / pattern) on every piece that exposes those groups. Heavy legs + a dress torso just adds. Matching meshes share a trimsheet and paint groups.

If a piece cannot take loadout paint, mark `paintLocked` in the shop. Default is unlocked.

---

## Rig truth (what we actually do)

One **humanoid biped**. Not three games.

| Bit | Shared? | Why |
|---|---|---|
| Spine, shoulders, thighs, upper arms | **Yes.** One skeleton. | Robots are this plus a different skin and unlocked joints. |
| Hips | Shared bone. Wider cat mesh + **Tail** socket. Tail mass is the CoM shift. | Pants share if they include the sealed port. |
| Feet | Same plantigrade-ish walk. **Tail is the difference.** | Bipedal. Tail counterweight, CoM a bit aft. Human-cousin cycle + hip/spine lean. Long-paw mesh still consult-only. |
| Head | **Not shared.** | Head picks the plan. |
| Hands | **Not shared.** Six vs five vs clamp. Cosmetic slot. | Arms shared above the wrist. |
| Extra arms / pods | Off by default. Machine piece or extra-limbs pack turns them on. | Same spine. Not `plan: Spider`. |
| ROM | Meat clips. Machine **may** ignore elbow/neck/spine limits. | A flag on the plan, not a new rig. |

Robot = human silhouette, machine materials, optional sockets, unlocked ROM. Cat = human silhouette + tail + paws + six fingers + ear head. Torso still swaps.

If a piece does not touch tail / ears / sixth finger, it goes in the shared bin. Boots stay shared until the foot consult says otherwise.

---

## Body plan first (this is the actual problem)

Species is not a texture. It is a **rig + socket list**.

| Plan | Skeleton | Extra sockets | Notes |
|---|---|---|---|
| `Human` | Standard biped | TailPort (sealed) | Port exists so cat pants work on humans closed |
| `Cat` | Same biped retarget, different hips / feet / skull | Tail, EarL, EarR | Lore: wider hips, paw feet, top ears, spine-width tail |
| `Machine` | Biped or close | none of Tail/Ear, Head *is* the helmet | Ryko-class not an MP shop item unless a lobby allows a shell |

**Head picks the plan.** Equip a cat head → pawn is `Cat` (ears, tail socket on, racial crumbs). Equip a machine head → plan `Machine`, tail/ear sockets off, helmet fused. Swapping head is a species swap; other pieces must pass compat tags or they grey out.

Gender / body base is a **compat tag**, not a third species.

| Tag | Who | Why it exists |
|---|---|---|
| `masc` | Human meat chest / some cat sliders | Muscle read |
| `fem` | Human meat chest / some cat sliders | Soft read |
| `nb` / `andro` | Own folder **and** reused on less-muscular masc chests | Saves a torso sculpt. Goes hard on some armor cuts |
| `none` | **Machine only** | Gender is meat. Chassis does not get F/M/NB folders |

Pieces list `body: [masc, andro]` or `body: [any-meat]` or `body: [none]`. Annoying tags beat a combinatorial mesh. Andro chest sharing onto soft-masc is the time-back.

`Machine` still has no gender channel. Fake hunk / fake chest on an android is an **under-armor silhouette skin**, not a gender folder. No organs, so silhouette is fashion. Not gacha. Paid or unlock, one price, you own the mesh.

Cyborg is not a fourth plan. It is a **piece tag** on a slot. Human with Longfall legs is still plan `Human`; the Legs skin is `legs_human_borg_longfall`. Cat with the same joke is `legs_cat_borg_longfall` — almost the same mesh, different boot last and hip cut (paw vs shoe, tail tunnel vs sealed port). A modeler duplicates and slices. They do not invent `plan: Cyborg`.

Weight lives **on the piece**, Helldivers-style. There is no global class that paints every slot.

```
legs_human_borg_longfall
  plans: [Human]
  body: [any-meat]
  weight: Heavy
  attach: [calfRail, ankleJet]
  crumbs: { speed: -0.08, plateHp: +0.15, hipDrain: +0.05 }
  optionalStatus: FleshBorged on Legs
```

Sum the crumbs. Two heavy legs + light torso ≠ “heavy class.” It is just the math. Attach points on a piece are where holsters / packs / autofense bolt on. No attach = that gun does not live there.

Racial crumbs (from **head only**, so you cannot stack three species):

| Head | Sauce / senses |
|---|---|
| Cat | Burst speed +, hip / stamina drain +, flash duration −, night vis +, motion blur off |
| Human | Baseline |
| Machine | Hip drain 0 (never gases). Move sauce −. No flash-as-flesh. Zap / Amped legal |

---

## Slots (closed)

Every plan has these four. Missing = hidden in UI, not a hole in the pawn.

| Slot | Human | Cat | Machine |
|---|---|---|---|
| Torso | yes | **Fully interchangeable with Human.** Same torso meshes. Tail port sealed on humans. | Own machine torsos. Meat jackets do not go on a chassis. |
| Arms | yes | yes (six-finger gloves if the hand is still meat) | Own. |
| Legs | yes | Own hips/feet/tail tunnel. **Depends.** See mix rules. | Own. **Depends.** |
| Head | helmet + face | helmet with ear holes + face | head unit fused **or** a helmet picked after race |

### Mix rules (locked 2026-09-16)

| Slot | Human ↔ Cat | Meat → Machine piece | Machine → meat piece |
|---|---|---|---|
| **Torso** | **Full swap.** One library. | No. Chassis chest is a core. | No. |
| **Arms** (stop at wrist) | **Same library.** | Meat may wear robot arms. | No meat arms on robots. |
| **Hands** | **Not shared.** Two cosmetics per meat race. | Optional borg hand if wrist matches. | **One** machine hand. |
| **Helmet** | Race filter first (ear holes vs none vs fused). After that, pick any helmet tagged for that plan. | Rare. Only if the head unit accepts a shell. | Machine head is not a human hat. |
| **Legs** | **Depends.** Paw last + tail tunnel vs shoe + sealed port. Author per pair. Do not promise a shared legs library. | Borg legs on meat: yes if hip socket matches (`legs_human_borg_*` / `legs_cat_borg_*`). | Meat legs on a chassis: no, unless a one-off “sleeve” piece is tagged for it. |

Helmet minors are small on purpose: breathing **Filter**, anti-flash glass (`Mirror` / flash duration −), Open face. Not a core. Not martyrdom. Race picks the hole cut; the glass is the shop.

**Add-on sockets** (not armor weight):

- `Tail` — Cat only. Armor piece or bare. Human armor still has a closed port so the same pant cut exists.
- `EarCover` — Cat helmet accessory (don’t pierce; paint is weird per lore). Human: nothing.

You do not make “feet” a fifth armor slot. Feet live on Legs. You do not make “tail” a weight-class. Tail is a plan socket with optional skins.

### Hands (cosmetic slot, locked)

Cats have **six fingers**. That does not fork the arm library. Arms stop at the wrist. Hands are a separate slot with **no combat crumbs**.

| Id | Who | Look |
|---|---|---|
| `hands_cat_nails` | Cat | Smooth skin, painted nails, fingerless glove, cable run into the forearm (robot tendons or fashion — same geo). |
| `hands_cat_plain` | Cat | No nail paint. Full tac glove. Reuse on masc / andro / whoever. Hides the sixth finger under the glove if the sculpt is lazy; still six bones. |
| `hands_human_nails` | Human | Five-finger nails + fingerless. |
| `hands_human_plain` | Human | Five-finger full tac glove. |
| `hands_machine` | Machine | **One mesh.** No gender folder. |

Wrist socket is `Wrist`. Meat arms and borg arms both expose it. Swap hands without swapping the arm. Gloves do not write plate HP.

Cat hands write **Grip +** and **Use time −** (sixth finger, different anim). That is why the slot exists. Cosmetic nails vs plain does not change the crumbs. Borg-hand on a cat drops the six-finger grip bonus unless the clamp piece lists its own Grip crumb.

---

## Under-armor silhouette (money on the table)

Worn under the plate. Shape only. Does not change sockets.

- Meat: masc / fem / andro / soft-masc-reuse-andro. Fanservice cuts allowed if the armor still closes. Wah wah.
- Machine: hunk / flat / fake-chest / twink-frame whatever. No organs, so “anatomically incorrect” is a texture joke.
- Not a gacha. Not a loot box. Unlock or buy the mesh, it stays in the library.
- Does not write combat crumbs. Armor segments write crumbs. A huge chest skin does not give plate HP.

---

## How a loadout is stored

```
head: head_cat_default          # plan = Cat
bodyBase: andro
underArmor: sil_andro_a
pieces: { torso, arms, legs }
addons: { tail, earCover }
paint: { primary, secondary, accent, patternId }
corePerk, training, hip, pack
```

A skin is `pieces.legs = "Cat.Light.Legs.PawCut"` not a global ID `cool_pants_03` that then fails on humans.

If a workshop author makes a human-only jacket, it is tagged `plans: [Human]`. The cat shop never lists it. Shared torso jackets are tagged `[Human, Cat]` and must include the tail-port geo even if sealed.

---

## Asset names

```
{slot}_{plan}_{family}_{variant}
legs_human_borg_longfall
legs_cat_borg_longfall
legs_human_kdv_light
torso_shared_plate_heavy      # only if UVs and sockets actually match
head_machine_optic_a          # no _m / _f
```

`family` is the look (borg, kdv, dress). `plan` is who it sockets onto. If two plans need the same joke, you ship two files that share a trimsheet, not one mesh with a cursed morph.

Mix rule: a piece lists `plans: [Human]` or `plans: [Human, Cat]` or `plans: [Machine]`. Socket mismatch (tail tunnel on a human hip with no port geo) is a no. Borg legs on a meat torso is a yes — that’s the whole point of slots.

FleshBorged on the damage sheet is the *armor attr* for those legs (bleed down, Zap legal). The cosmetic file does not do Resolve. If you wear borg legs, the sheet can stamp FleshBorged on the Leg segments. Optional, not automatic, so a fashion prosthetic is not free ion vulnerability.

---

Avoid-list is in `design notes/README.md`.
