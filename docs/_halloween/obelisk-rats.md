# Afterlife obelisk-rats vs living-world obelisks

## Living-world obelisks (not these)

In the regular game, obelisks are technically just laser turrets in very tall watchtowers with cameras.

Do not reuse that prefab, that AI, or that name internally for the afterlife creatures without a rename in code. Players can still nickname the afterlife things “obelisks” because that is the 3D silhouette.

## Afterlife “obelisks”

Small four-dimensional creatures. They are the 4D equivalent of a rat. That is how bad / small they are in their native terms. You are not fighting a monument. You are being bothered by vermin whose true body you cannot see.

What you see is a 3D cross-section: a matte-black obelisk-like prism that absorbs light. Slightly too tall for a comfortable object, but still “small” compared to architecture. No spawn animation. It simply is behind you after you have been drifting long enough to relax.

They form out of leftover axioms: watching, judging, counting the time you have left. They belong to the thought-sludge. They are not imported living-world hardware.

### Core behavior (main void)

After a quiet stretch, one appears behind the player and silently matches movement, staying just outside peripheral vision. Subtle tells: fog feels wrong, blood-layer footprints stop filling in quite right.

When brought into view:
- It locks in place.
- Deep stone-on-stone grinding noise that echoes too long.
- It does not immediately attack. It stands.

### Branch: turn away and keep moving

It follows again, now with a continuous directional grind. It does not close distance unless you stop completely.

Facing away while repeatedly keeping only one in vision, as new ones become the pursuer, makes the current pursuer slightly faster per extra “second obelisk” that has been promoted into the chase.

### Branch: keep it in view and move “forward”

The void ahead becomes a long straight corridor of featureless geometry. Eventually a second one blocks the way.

If you turn around to look at the first again, the first despawns from the old position and reappears behind the second. The second is now the locked one. The cycle can repeat. Corridors can feel slightly narrower; grind can feel denser.

### Branch: get both in view at once

The farther one undergoes a silent spatial inversion and becomes the walls / floor / ceiling of a closed room of the same matte material. Blood layer remains underfoot but gravity can feel wrong.

The nearer one accelerates in a straight line. No roar. Sudden silent velocity.

Parry / successful response:
- Player sees themselves standing in the center of a ritual circle.
- Something is granted (stabilizer fragment, clearer navigation, reduced merge pressure, or a Babel page that actually resolves — pick in implementation).
- Then they are briefly beamed back down, intersecting reality and emitting plasma. See `exits-and-plasma.md`.

Dodge / fail to parry the rush:
- Dropped into a foggy, wet sub-plane: one big obelisk-rat and many little ones that chase while you parkour to the big one.
- Reaching the big one returns you to the main void or grants a lesser reward.
- Dying in that maze ends the afterlife instance and uses ordinary respawn (print back / return to a body that has your brain).

Ryko note: the dual-texture blade phases through the first one more easily, which can cause the second to form faster or leave chip/brain smears on an inverted room.
