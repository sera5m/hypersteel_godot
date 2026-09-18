# Loadout cards (concept test)

UI filter: **race + body-base** first. Shop only lists pieces tagged for that plan. Internally Human.Heavy.Torso ≠ Cat.Heavy.Torso even if they share a trimsheet. Filter is how the mess stays off-screen.

Campaign freaks are not the MP shop. Ryko’s card exists so we can see which rows are *her sheet* vs rows an operator is allowed to copy.

---

## Ryko (campaign pawn, not a shop preset)

| Slot | Equipped | What it actually is |
|---|---|---|
| Plan / head | Machine (full android). Five-eye skull is **hers**, not an MP head | Species: stamina = energy, sauce default −, Zap/Amped legal. The extra eyes are a campaign mesh. |
| Under-armor | n/a — she *is* the shell | |
| Torso / core | **Nuclear cannibal** | Baked: kill-to-heal / mass siphon (campaign loop). Plate attrs: **MultiLayerComposite + NanotubeFiber + MetalFoamPadded**. Third attr is half by stack rule — Foam is the half, or drop one. |
| Arms | **Quick draw** | Baked: holster/draw time → 0 (pre-sequences + microjets). Not a new gun. |
| Legs | **agility_android_lean** | Baked: +25% wish-speed, **no sprint state**. Dash still lives on the hip. |
| Helmet | part of the head unit | Machine: fused. |
| Hip | Jumpkit, dash, tank **3** | Existing JumpKitDef. Kit **nulls fall damage** and gives air steer. Legal hip. |
| Pack | **Extra limbs** | Shoulder pods / extra arms. Grenade + pack verb do not occupy the gun hands. Recoil −50%. This is a pack that *is* geometry. MP may get a weaker cousin; her two extra arms + pods are campaign. |
| Left | Anime sword | Melee packet + parry on the shared bar (movement doc). |
| Right | Campaign gun list | Not an MP arsenal dump. |
| Picked perks | She has **three**. Operators get **one**. | **Packing heavy**: two power weapons + up to 6 guns, −33% speed of *base* (before lean legs). **Rapid regen**: heal rate on the sheet (already 1000/s lore). **Bloodlusted**: kill / damage → short Stimmed/Amped / Redline crumb. |

Math check: lean legs +25% wish, packing heavy −33% of **base**. Net is not “she is slow.” Apply tax to the species base, then add lean. Write it that way or the perks fight.

What operators **cannot** copy from this card: nuclear cannibal as a shop core, five-eye head, full extra-limb rig, three perk rows, the campaign gun wall.

What they **can** echo: lean-legs piece, quick-draw arms, jumpkit tank 3, packing-heavy as a *picked* perk, bubble/limbs as packs.

---

## Crusher (MP operator)

UI: Race **Human**, body-base **masc**. Shop greys cat-only and machine-fused heads.

| Slot | Equipped | What it actually is |
|---|---|---|
| Plan / head | Human [masc] | Species EMS-proof-ish. Body-base: **+8% HP, −6% agi, +4% spd, +5% worse heat**. Machines have no gender row. |
| Torso / core | **Bronze brawns** Heavy | Plate attrs: **SolidMetal + Mirror + ShearThickPadded**. Legal three; third is half. Baked minors: none required. This chest *is* the tank numbers. |
| Arms | **Heavy hitter** | Baked: melee packet +40%, melee windup +20%. |
| Legs | (unset in your list — default heavy to match the chest or mix) | If unset, Human.Normal.Legs. Do not silently inherit Bronze brawns onto legs. |
| Helmet | unset | Can take plate. Cannot bake Nuh uh. |
| Hip | default jumpkit unless they picked battery | |
| Pack | **Plasma bubble** | Locked shield rules. Lightning feed/charge still works. |
| Left | Explosive-head sledge | Melee + Explosive splash on connect. |
| Right | Special AT: RPG revolver. Normals: X12 marksman, lightning gun | Special slot + two normals is an MP weapon rule, not a perk. Packing heavy is how you get two specials. Crusher has one special. |
| Picked perk | **Didn’t hear no bell** | **Nuh uh.** 0 HP → crit band, no bleed-out. Second 0 dies. Executions count. |

No second pick. Heavy hitter is baked on the arms, not the pick. Bronze brawns is stats + attrs, not a perk name.

---

## Why this un-messes the UI

Player sees, in order:

1. Race + body-base (filter)
2. Head / torso / arms / legs / helmet (meshes that survived the filter)
3. Paint (loadout layer)
4. Hip, pack
5. One picked perk
6. Training/HUD if we keep that row
7. Guns

They never see `plans: [Human]` tags. They see “Human [masc]” and a list that already fits.

Internally every piece still has `plans` + `body` + baked perks + crumbs + plate attrs. The filter is the design.

---

## Flags from the test

- Ryko perk count is a campaign exception. Do not let MP buy three.
- Packing heavy taxes **base** speed so lean legs still matter.
- Composite + nanotube + foam on her chest: two full, third half. Document which is half on the piece.
- Male +5% HP / −8% speed is a **body-base crumb**, not a perk. If nb/andro exist to reuse the chest sculpt, give them their own tiny crumb or zero, not “secret female tax.”
- Crusher needs explicit legs or the card is lying about being a full heavy.
- Extra-limbs pack is the only pack that changes the *rig*. Treat it as a pack with extra sockets, not a fourth species.
