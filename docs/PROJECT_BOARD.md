# Hypersteel project board

Twin of the `hypersteel-project-manage` skill. Status only. Design stays in the other docs/*.md files.
Repo `sera5m/hypersteel_godot`.

Tokens — done | almost | tweaks | wip | 35% | tbd | not started

## Movement — almost
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts/Abilities/Kits/JumpKit
Interacts with — Entity motor slot, FSM, JumpKit. Depends on — CharacterBody3D, first-party Jolt.
- Source ground/air almost
- Sauce Resource done
- PlayerMovement partials done
- FSM verbs done as impulses
- JumpKit box + IMoveMotor + compose flag wip
- PlayerMovement does not yet honor GravityScale / skip Source during dash lock
- In-game tune tweaks

## Damage / Health — almost
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts/Damage
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts/Health
Interacts with — Entity, Status, Feelings, future weapons.
- Limbs done | Types/packet/AP done | Status tweaks | Composition done
- ArmorPiece + DamageReceive almost | HurtBox scenes tbd | Dummy done | Flavor text tbd

## Entity — almost
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts/Entity
- ActorEntity door done | KitComposer done | pawn stubs done
- Dummy receives through armor done
- Player scene still PlayerMovement root — wip
- jumpKit slot on ActorEntity wip

## Status / feelings — tweaks
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts/Damage
- Tags almost | Flinch almost | DoT/heat tweaks | Morale not started

## UI — unfinished, 35%
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts
- SP HUD — debug labels only
- MP HUD — not started
- Damage/limb UI — not started

## Weapons — not started
Add `Scripts/Weapons/` when begun. Depends on Damage door.

## AI — not started
https://github.com/sera5m/hypersteel_godot/tree/main/Scripts/Entity/NpcAI
- Soldier Critical stub done | Dummy done | roster law done (`design notes/21-enemy-roster.md`)
- BT / perception not started (order: dummy soldier shoots/flinches/dies → scout alert → squad leader coord)
- Valkarie / morale not started

## Audio / VFX — wip
Legacy player audio/particles exist. Damage impact sounds tbd.

## Net — not started

## Focus
Put an ArmorPiece collider on Dummy in a scene and fire `DamageProbe.TryHurt`. Then weapons. Do not grow PlayerMovement.cs.
