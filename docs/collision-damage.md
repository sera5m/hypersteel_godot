# Collision damage

Jolt shoves. This component **watches** the contact and waits.

1. First frame of a pair → open a watch (`RelBefore`, peak closing speed).
2. While still sliding against that rid → update peak close. No HP.
3. Pair gone from `GetSlideCollision` (separated) or `MaxWatchSec` → measure
   `max(0, closeBefore − closeAfter)` — the speed Jolt actually removed.
4. One blunt packet on **this** body. The other body, if flagged, does its own watch.

`DoesBluntForce` off = prop / bullet, no HP from this path.

`kinetic = inelastic(Δv) * cos²θ * stance * footing * (1 - resist)`
