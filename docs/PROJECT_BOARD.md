# Hypersteel project board

Twin of the `hypersteel-project-manage` skill. Status only.
Repo `sera5m/hypersteel_godot`.

Tokens — done | almost | tweaks | wip | 35% | tbd | not started

## Movement — almost
- Source ground/air almost | sauce done | PlayerMovement partials done
- FSM slide/vault/wallrun/ladder done as impulses
- JumpKit + WallRun + Slide boxes almost
- PlayerMovement does not honor GravityScale / skip Source during dash lock
- In-game tune tweaks

## Damage / Health — almost
- Limbs / packet / AP done | plate soak almost | HurtBox scenes tbd
- Dummy done | Status tags almost | DoT/heat tweaks
- Packet now carries ApplyStatus from gun kit

## Entity — almost
- ActorEntity + KitComposer done
- EntityStats on every actor (Mass, LiftKg, Strength, Agi…) done
- Ryko 600 kg lift / 220 loaded; Enduring 1000 kg lift stub
- Throw = mass ≤ 50% of lift left after hold
- Player scene still PlayerMovement root — wip
- PawnActions + OperatorInput exist; not dropped on the parkour scene

## Weapons — wip
Was "not started". Now:
- Gun + SightDef + Attach slots + GunKit handling vs Strength done
- TestRifle cheat fire/reload/alt done
- Hands law 24: LMB fire, RMB alt, MMB ADS hold/toggle, E context
- Catalog mags/stocks/muzzles per ammo family done
- Named roster guns (Kar8, ISL, Volcano…) not actors
- Recoil into SourceMove tbd | inspect UI tbd | project.godot action map tbd

## AI — wip
- NpcBrain four-clock + Soldier + sense/cover/reflex LAND
- Fire / reload / ADS-at-far go through PawnActions
- Turn tax uses EntityStats.LiveTurnRate(gun.TurnSlowdown)
- Mesh consensus off | bounce-guess stub | Valkarie not started

## Status / feelings — tweaks
- Tags almost | Flinch almost | ApplyStatus on packet not consumed by StatusComponent yet

## Weather — almost (code tree)
- WeatherManager + bake wind map + planet presets on repo
- Particles / per-level volumes still art

## Mechs — design only
- 07-mech fused chassis + cores + pods. No walker pawn.

## UI — 35%
- Debug speed labels only

## Audio / VFX — wip (legacy player)

## Net — not started

## Focus
Drop OperatorInput + PawnActions + Gun on a scene that is ActorEntity. Consume ApplyStatus in StatusComponent. Do not grow PlayerMovement.cs.
