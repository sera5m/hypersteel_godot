# Nav profiles and the SETUP layer

Differentiated meshes live in the level. Baking is a tool pass, not gameplay code.

## Why several meshes

Godot Recast is Y-up. `agent_max_slope` (default 45°) means “still a floor.” Cranking it toward 90° does not produce wall-run geometry. Different actor types already mean different baked meshes + layers (or maps). Walk vs wall vs leap is the same pattern as infantry vs tank.

Do not fork the engine baker. Wrap it.

## Parse once

```
source = NavigationMeshSourceGeometryData3D
parse_source_geometry_data(walkProfileMesh, source, levelRoot)
```

Reuse `source` for every profile. Godot documents this: parse is the expensive SceneTree walk; bake can run N times from the same blob.

Prefer physics / parkour collision as source, not visuals.

## Profiles

`HypersteelNavProfile` (Resource):

- `name` (`walk`, `wall`, `leap`)
- `kind` = Walk | Wall | LeapLinks
- `navigationLayers` bitmask
- `agentRadius` / `agentHeight` (Walk)
- wall: max up-dot to accept a face, min patch size
- leap: max gap metres (4), max height delta

`HypersteelNavBake` (`@tool` Node3D) in the **SETUP** layer of the chunk:

1. Parse once.
2. Bake Walk → `NavigationRegion3D` layer `Walk`.
3. Split vertical faces, bake each patch as a floor in wall-space → regions layer `WallRun`.
4. From Walk rims, emit `NavigationLink3D` layer `Leap`.

Play mode does not rebake unless a streamed chunk arrives.

## What sits in the level

```
LevelChunk/
  SETUP/
    NavBake
    Nav_Walk          (region, layer Walk)
    Nav_Wall_01..N    (regions, layer WallRun)
    Nav_Leap          (links, layer Leap)
```

Name the bits in Project Settings the same way as physics layers: `Walk`, `Leap`, `WallRun`, `Vault`.

## Not this

- One mega mesh with slope 89°.
- Runtime A* on triangles of the render mesh.
- Valkarie flight baked here.
