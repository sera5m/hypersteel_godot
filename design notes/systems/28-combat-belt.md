# Combat belt / item interaction

Locked shape 2026-11-21. Companion to the storage pouch / inventory bag (Tab).
Inspired by Helldivers 2 item-switching Combat Belts. QoL for small tools that are not guns or packs.

Does not invent new Resolve branches or pawn systems. Belt is data on the inventory Resource.

---

## Why

Certain tools (C4 triggers, small missile targeter, flares, similar quick-use items) do not belong buried in the pouch. They live on the belt for fast access. Remaining items stay in the storage pouch.

---

## Belt

- Exactly **4 slots**.
- Separate fixed container from the main inventory list.
- Items may be moved between belt and pouch at any time (inventory UI or drag).
- Belt items are available for item-switching without opening the full bag.

---

## Eligibility

Simple flag on the item inventory def:

```
has_belt_clip = true   // can go on belt or stay in pouch
has_belt_clip = false  // pouch only (most items)
```

- Only items with `has_belt_clip` may be equipped to the belt.
- Many inventory items cannot go on the belt. The flag is the gate. No size or type exceptions beyond the flag.
- Default for the listed tools (C4 trigger, missile targeter, flares): `has_belt_clip = true` and start on the belt.

---

## Rules

- Belt capacity is hard-capped at 4. Overflow stays in the pouch.
- No auto-sort. Player chooses which eligible items sit on the belt.
- Using a belt item consumes or activates it the same way a pouch item would. Location only affects access speed and UI.
- Death / drop: belt contents follow the same MP death rules as the pouch (see future throwables note).
- AI may ignore the belt distinction; they use the same item defs.

---

## Not this

- Not a second inventory system.
- Not a weapon slot or pack slot.
- Not a place for guns, melee, or hip/pack appliances.
- Does not change ammo sharing or bag capacity elsewhere.
