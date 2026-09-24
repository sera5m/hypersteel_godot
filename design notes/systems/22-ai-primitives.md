# AI primitives

Node-equivalents. Leaves of the four clocks in `16`. Not a second brain. Same packets, same Status, same RoleDef.

Execution picks a primitive. Reflex may interrupt it after the fact.

## Combat aim

Confidence C in [0,1] gates which primitive is legal. Anxious bounce-around-corner needs high anxiety or high C that they are still there.

| Primitive | When | Notes |
|---|---|---|
| AimHitscanTrack | Hitscan. Turn rate limited. | Aim at current body. Track lag = turn rate vs angle error. |
| AimLeadFlat | Projectile, no drop (or drop ignored). | Constant-velocity intercept. |
| AimLeadBallistic | Projectile with fall. | Intercept time + aim above by drop. |
| AimLeadAccel | Fast target, intelligence high. | Newton on t if accel known. Else AimLeadFlat. |
| AimBurst | Rifle meat default. | Same aim primitive, fire n then pause. |
| AimPeekCorner | Later. | Not this week. |
| AimBounceGuess | Anxious or C high that target is behind that wall. | Only if the gun ricochets. Same bounce as the player projectile. |

Range switch is inventory + RoleDef bands, not a class. Scout close <= 5m. Soldier mid ~8-25m. Sniper close <= 10m then sidearm.

### Intercept (flat lead)

R = P - S. Need t>0 such that |R + V t| = s t.

a = V.V - s^2,  b = 2 R.V,  c = R.R

a t^2 + b t + c = 0. Take smallest t>0. Aim point = P + V t.

### Ballistic drop

Same t, raise aim by 0.5 g t^2. Use the gun's g.

### Accel / arcs

P(t) = P + V t + 0.5 A t^2 makes |P(t)-S| = s t a quartic. Newton from t0 = |R|/s. Two or three steps. Diverge -> flat lead. Conscripts never run this.

### Confidence

C = clamp(vis * exp(-t/tau) * (1 - theta/thetaMax) / cover, 0, 1)

## Traps / doors

Door/exit = nav portal. PlaceAtPortal. Own traps: team id, step-over no friendly detonate. StickMunition, CutReel, DropBlocker, RaiseCover. Int gate.

## Combos

High Int or Agility may chain official Status from 15. No AI-only combo packet.

## Stats on AI

Same fields as MP (10). Officer fear resist up. Conscript down. RoleDef engage bands.

## Morale states

Terrified: GetOutOfView / leave-radius. Enraged: reckless MoveAndAttack, mild stim crumbs, red eyes (art later). Neutral: RoleDef. Ally Died/glory -> empathy. Kill -> short Joy (morale, not heal).

## Nav primitives

CheckRoom, BackUpRadius, Rush, Defend, FallBack, Retreat, Cover, HalfCover, ZigZag, LeaveToRing, HoldBand, Vantage, BackOutOfView.

Last known: position + velocity + accel. Dead-reckon until stale.

Tiny ToM: ISeeThem / TheySeeMe notify only. Vanished at door -> wait portal or bounce-guess. No deep planner.

## Team

Off unless TeamComms module on. Rank int: higher obeys. Same rank: role-weighted vote. HelpDying if Int + comms + not Terrified. Mesh off until Soldier walks.

## Build order

1. [x] Soldier walk + last-known + AimHitscanTrack / AimLeadFlat.
2. [x] HoldBand + range switch.
3. [x] Terrified / Enraged hooks.
4. [x] Cover + BackOutOfView.  (Cover EQS-lite + forward hazard ray in TickSense 2026-09-24; BackOutOfView via GetOutOfView + hazard rewrite)
5. [x] TeamComms + rank + HelpDying stim (17).  (Sense.HasStim + HelpDying prefers stim then ally; SeekWorld stim when HP low 2026-09-24)
6. Traps / bounce / accel lead.
