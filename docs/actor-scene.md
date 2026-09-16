# How to put a body on an actor (editor, not code)

The visual editor **is** the Godot scene tree. Open `Scenes/Entity/Dummy.tscn` and look at the left dock. That tree *is* the dummy.

```
Dummy          CharacterBody3D + DummyPawn   world position, walks, floor/wall hull
├ Hull         CollisionShape3D (capsule)    physics only — PawnPhys
├ Visual       instanced .fbx / .glb         what you SEE (Skeleton3D lives in here)
├ Health       HealthComponent               HP / limbs. Not armor.
├ PlateChest   ArmorPiece                    soak, then remainder to Dummy.Hurt
│   └ Hit      Area3D + box                  Damage layer only
└ Controller   later: AI / none / player
```

## Drop a skeleton on it

1. Open `res://Scenes/Entity/Dummy.tscn`.
2. FileSystem: `assets/charachters/ryko_pre_tweak_human/SKM_ryko.fbx` (already wired) **or** your Quaternius UAL `.glb`.
3. Drag the `.fbx` / `.glb` onto the `Visual` node (or replace `Visual`).
4. Click the imported child → you should see `Skeleton3D` + meshes. That is the skeleton view.
5. Select `Health` → Inspector → `Skeleton Path` = `Visual/.../Skeleton3D` so bone hits map to limbs.

Quaternius Universal Animation Library: after AssetLib install, the characters live under whatever folder you extracted (often `addons/` or `assets/`). Drag **one** character scene onto `Visual`. Idle: add `AnimationPlayer` as sibling, pick the library idle clip.

## Armor is a child object, not health

- `ArmorPiece` soaks, then **forwards leftover kinetic** to the parent `ActorEntity`.
- Do not put HP on the plate and call that the dummy.
- Parent the plate to a `BoneAttachment3D` on `chest` / `spine` if you want it to follow the mesh.
- Plate collider = Area3D, Damage layer, mask 0. Hull capsule stays on PawnPhys so plates do not fight walking.

## Every actor (player, dummy, mech)

| Need | Node |
|---|---|
| Takes damage | ActorEntity + Health + optional ArmorPiece children |
| Moves | CharacterBody3D |
| Controller | player script / AI / none |
| Visible body | instanced mesh under Visual |
| Floor / walls | Hull CollisionShape3D |
| Hitboxes | HurtBox / ArmorPiece Area3D on Damage |
