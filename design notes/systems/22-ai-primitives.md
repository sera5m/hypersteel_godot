# AI primitives

Leaves of the four clocks in `16`. Not a second brain.

## Perception board

`NpcTargetBoard`. Wide cone = peripheral (`InWideCone`). Aim cone = **half of wide** (gun can point). Slack 30 deg = fire with no body turn, inside the aim cone. `TheyFaceMe`: unobstructed and their look within 45 deg of us.

Target: Body, last pos/vel/accel, danger, InWideCone, InAimCone, TheyFaceMe.

Subjective: Priority accumulates (bigger = more) when they damage this pawn or allies. Distance, range-band, HP%.

Affinity vs preferred radius R: -25% per R outward, -12.5% per R inward. Score = priority * affinity.

Smart (`NpcStats.SmartPriority`): decay if unseen and has not hurt me lately; bonus if occupant HP < 6.5 * gun damage.

Ally list: ref, pos, dist, HP, rank, stim/ammo flags. Callout writes last-known onto squad boards. Meat may consensus later. Drones obey rank or wander to last callout.

## Combat aim

AimHitscanTrack, AimLeadFlat, AimLeadBallistic, AimLeadAccel, AimBurst, AimPeekCorner later, AimBounceGuess later.

## Traps / doors

NpcTrapPortal stub + nav_portal. No spawn yet.

## Morale / nav / team / build order

See prior checkmarks. Mesh off. Valkarie empty.
