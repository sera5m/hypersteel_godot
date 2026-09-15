# Collision damage

Jolt shoves. This component writes blunt HP **only onto its own pawn** after the contact exists (`GetSlideCollision`). The other body, if it also `DoesBluntForce`, applies its own packet. Same source never hits twice (no GMod collider loop).

`DoesBluntForce` lives on `CollisionDamage` (anything with a move body). Ballistic projectiles leave this off.

## Scale

`kinetic = inelastic(m, M, vn, e) * cos²θ * stance * footing * (1 - resistBlunt)`

- `e` flesh 0.18, steel 0.58. Higher e = more bounce already in physics, less HP.
- `cos²θ` = `(v·n)² / |v|²`. Head-on 1, scrape 0. No acos.
- Stance: air 1, climb 0.7, wallrun 0.6, ground 0.5, crouch 0.2, prone 0.05.
- `Footing` / `ResistBlunt` are per-character.
- Wallrun + enough kinetic → `BluntKnockOff`. Huge mass gap → `BluntRagdoll` or `BluntFlinch`.

FSM should `SetStance`. Fallback is floor vs air.
