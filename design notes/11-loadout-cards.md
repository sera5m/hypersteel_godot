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
| Arms | **Quick draw** | Baked: holster/draw time → 0 (pre-sequences + microjets). |
| Legs | **agility_android_lean** | Baked: +25% wish-speed, **no sprint state**. Dash still lives on the hip. |
| Helmet | part of the head unit | Machine: fused. |
| Hip | Jumpkit, dash, tank **3** | Existing JumpKitDef. Kit **nulls fall damage** and gives air steer. |
| Pack | **Extra limbs** | Shoulder pods / extra arms. Grenade + pack verb do not occupy the gun hands. Recoil −50%. Geometry pack. MP may get a weaker cousin. |
| Left | Anime sword | Melee packet + parry on the shared bar. |
| Right | Campaign gun list | |
| Picked perks | She has **three**. Operators get **one**. | **Packing heavy**: two power weapons + up to 6 guns, −33% speed of *base* (before lean legs). **Rapid regen**: heal rate on the sheet. **Bloodlusted**: kill / damage → short Stimmed/Amped crumb. |

Apply packing-heavy tax to species **base**, then add lean legs.

Operators can echo: lean-legs, quick-draw arms, jumpkit tank 3, packing-heavy as a picked perk, bubble/limbs packs.

---

## Crusher (MP operator)

UI: Race **Human**, body-base **masc**.

| Slot | Equipped | What it actually is |
|---|---|---|
| Plan / head | Human [masc] | Body-base: **+8% HP, −6% agi, +4% spd, +5% worse heat**. |
| Torso / core | **Bronze brawns** Heavy | Plate attrs: **SolidMetal + Mirror + ShearThickPadded**. Third is half. |
| Arms | **Heavy hitter** | Melee packet +40%, melee windup +20%. |
| Legs | unset — pick Human.Normal.Legs or a heavy pair explicitly | |
| Helmet | unset | Can take plate. |
| Hip | default jumpkit unless battery | |
| Pack | **Plasma bubble** | Locked shield rules. Lightning feed/charge works. |
| Left | Explosive-head sledge | Melee + Explosive splash. |
| Right | Special AT: RPG revolver. Normals: X12 marksman, lightning gun | One special. Packing heavy is how you get two. |
| Picked perk | **Didn’t hear no bell** | **Nuh uh.** 0 HP → crit band, no bleed-out. Second 0 dies. |

Heavy hitter is baked on the arms. Bronze brawns is stats + attrs.

## UI order

1. Race + body-base (filter)
2. Head / torso / arms / legs / helmet
3. Paint
4. Hip, pack
5. One picked perk
6. Training/HUD if kept
7. Guns

## Flags from the test

- Ryko perk count is a campaign exception.
- Packing heavy taxes **base** speed so lean legs still matter.
- Third plate attr is half; mark which on the piece.
- Body-base crumbs are not perks.
- Extra-limbs pack adds sockets. It is not a fourth species.
