# AI behavior

Not started in engine (`NpcAI` stub + soldier critical hook only). This is the law so trees don’t grow a second game.

AI uses the **same kits** as players: hip/pack or hull sockets, same packets, same Status, same `TryPulse`. No `if (titan)` in Resolve. No AI-only damage.

## Layers

| Layer | Whose job |
|---|---|
| **Perception** | See / hear / ping. Sensor pack and Radar pod write the same outline the player gets. |
| **Morale** | Encourage / Terrified. Not Health. |
| **BT / utility** | Pick a verb the kit already has. `TryPulse` on JumpKit / pack / pod / core. |
| **Feelings** | Flinch from `Damaged`. Optional. Drones off. |
| **Hooks** | `Died` / `Damaged` / `Band`. Same perk hooks. |

Projectile never talks to AI. `Damaged` signal → this pawn’s hook.

Roles are **kits**, not classes. Hull AI is the same BT with bigger sockets. Center-stage lock. Same item pool as `17`.
