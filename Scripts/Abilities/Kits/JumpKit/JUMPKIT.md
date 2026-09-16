# Jumpkit

Mounted appliance, same role as a plate. Not GAS. Not per-character.

Path: `Scripts/Abilities/Kits/JumpKit/`

## Pieces

| File | Role |
|---|---|
| `JumpKitDef` | Resource: capacity, costs, verb flags, dash 60 m/s / 0.2 s |
| `JumpKit` | Live box: bar, cooldown, pulse/hold, regen |
| `IMoveMotor` | Closed functions the kit may call |
| `CharacterBodyMoveMotor` | Default map onto any `CharacterBody3D` |
| `JumpVerb` / `JumpTrigger` | AirJump, Dash, Hover, Slam, Flight — pulse or hold |

## Mapping

Owner (`ActorEntity` or any node with a controller) on compose:

1. Find or create `JumpKit` child if `KitSheet.hasJumpKit`.
2. `kit.BindMotor(motor)` or `kit.BindBody(characterBody)`.
3. Keep `entity.jumpKit` as the watch pipe.

Player HUD and AI both read `Current`, `Max`, `CanPulse`, `CanHold`, `IsBusy`, `TimeUntilReady`.
Controllers only fire `TryPulse(verb, wishDir)` and `SetHold(verb, held, wishDir)`.
Owner hooks call `NotifyLanded` / `NotifyHooked` / `NotifyKilled` / `AddFuel` / `Refill`.
The kit does not know Ryko or Valkarie.

## Motor surface the kit may use

`ApplyImpulse`, `OverrideVelocity`, `RestoreVelocity`, `SetAirControl`, `Velocity`, `Forward`, `OnFloor`, `GravityScale`.

Dash: snapshot velocity + wish, override at `dashSpeed` for `dashSeconds`, restore `preDash + dashDir`.
Hover / flight: hold trigger, drain while held, gravity scale from the def.
Slam: dump the bar, impulse along look.

## Compose

`KitSheet.hasJumpKit` + `KitSheet.jumpKit`. `KitComposer` spawns the child and binds the host body when the host is a `CharacterBody3D`.

Variants are defs:

- Plain: air jump only
- Dash (Ryko): air jump + dash, capacity +1, fast regen
- Hover / Slam: extra flags
- Winged (Valkarie): `hasFlight`

Do not subclass `JumpKit` per pawn. Do not put dash code in `PlayerMovement`.
