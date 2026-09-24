# Player controls

Default keyboard / mouse. Rebindable. Same verbs for campaign and MP.

Code: `PawnActions` (verbs) + `OperatorInput` (keys). AI calls PawnActions / Gun / JumpKit. Never Input.

ADS zoom is an optic. Menu: ADS hold or toggle (`OperatorInput.AdsToggle` / `PawnActions.AdsToggle`). Default hold.

## Mouse

| Input | Action name | Does |
|---|---|---|
| LMB | fire | Primary |
| RMB | alt_fire | Alt on this gun. Not ADS |
| MMB | ads | ADS hold or toggle |
| Scroll | weapon_next / weapon_prev | Slots |
| T | paint | Paint target |

## Hands

| Input | Action name | Does |
|---|---|---|
| E | interact | Context |
| F | melee | Left hand |
| Q | grenade | Grenade |
| R | reload | Reload |
| X | sling | Gun away |
| Tab | inventory | Bag |
| I | inspect | Inspect |
| U | configure | Attach popup |

## Move / kits

| Input | Action name | Does |
|---|---|---|
| Space | jump | Jump (PlayerMovement already) |
| Shift | jumpkit | JumpKit pulse |
| Ctrl | crouch | Crouch if under 4 m/s, else slide |
| Alt | prone | Prone |
| C | boost | Boost |
| Caps Lock | ability | Backpack |
| Z | callin | Call-in menu |
| 1-6 | slot_1..6 | Weapons |
| Y / Enter | chat | Chat |
| F1 | emote | Emote wheel |

Built today: fire / alt / ads / reload / mode-via-E / jumpkit / sling / inspect on OperatorInput. Melee, grenade, call-in, chat, emote, inventory UI, paint, prone not wired. PlayerMovement still owns WASD / jump / crouch-slide FSM and is not ActorEntity yet.
