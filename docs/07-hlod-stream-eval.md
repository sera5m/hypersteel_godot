# HLOD + stream + tick — evaluation (do not modify Chunx yet)

Stitch of three prompts:

1. Chunx cells + DH-style queue + decimated meshes (not paintings) for mid/far.  
2. Nanite ideas only as *static rungs* + tick emulation (skip-ahead transforms).  
3. Named collapse rules + thin/small-% geo flattened to atlas pixels.

Goal of this note: decide if the system is worth a baker + observer, or if Hypersteel should stay “chapter scene + a few layered packeds.”

---

## 1. The system in one page

Each **cell** (authored chunk: barracks, vault slice, pipeworks bay, ridge segment) produces four products at bake time:

| Product | What it is | When shown |
|---|---|---|
| `Gameplay.tscn` | Real nodes, scripts, lights, full colliders | Touch radius |
| `LodMesh` | Merged QEM + **named primitives** (pipe→tube, beam→box) | See-but-not-enter |
| `LodAtlas` | Albedo+normal of **thin / cheap-%** junk | Mid + far trim |
| `LodHull` | One (or few) collision meshes for legal wallrun/land | Mid if she can still hit the shell |

Runtime observer on camera (hysteresis volumes):

- Near: instance Gameplay; hide LOD. Full tick.  
- Mid: free Gameplay; show LodMesh + atlas; hull only if the cell is a legal surface. Cheap tick or skip-ahead.  
- Far: Lod2 / atlas / cell HLOD. Skip-ahead or frozen.  
- Out: free all; optional `{pos, vel, nextEventTime}` stub if something must fire later.

Promotion (she dashes at a mid cell): threaded load Gameplay, overlap frames with LOD still up, then hide LOD. Fail toward real geo.

Bad-PC profile = same bakes, nearer swap distances, flatten threshold looser (30cm / higher %).

---

## 2. Expanded rules

### Named collapse (semantic LOD)

Tags on MeshInstance / parent:

- `Hero` — never below hull. Wallrun, doors she uses, climb pipes she can grab.  
- `Pipe` / `Cable` — N-gon prism along spline (3–8 sides by rung).  
- `Beam` / `Girder` — OBB.  
- `Tank` — cylinder / capsule.  
- `Facade` / `Panel` — 1–2 quads + baked normal.  
- `Decimate` — fallback QEM.

Swap by **screen error** (projected height in px), not only meters. A 2m pipe and a 40m main do not share “100m.”

### Thin → pixels

After heroes and named tags:

Flatten into `LodAtlas` if **either**:

- longest AABB axis `< 20cm` (compact junk: bolts, needles, frames), **or**
- remaining tris or visual area is `< ~2–5%` of the cell (noise on a facade).

Do **not** flatten a 19cm × 80m pipe — that is `Pipe`. Longest-axis vs percent: spaghetti uses named collapse; compact uses pixels.

Never flatten a hole she can dash through. Grate gaps stay mesh or hull-cut.

### Tick emulation

```
important = can hurt player now (LOS, fuse, door in use, mounted hull)
visible   = in camera + near/mid gameplay

if important: real _PhysicsProcess
elif visible and mid: tick every 3–5 frames
else: pos += vel * skipped_dt; advance spline; no process
```

Looking back does not replay 10 frames. Snap to t+10. If `nextEventTime` is “shoots at player,” promote cell.

### Nanite: steal / don’t

Steal: density is authorable; screen-space error; coarse-first fill (Lod2 every cell, then Lod1, then Gameplay).  
Don’t: runtime cluster DAG, visbuffer, “thin geo stays high-poly,” generating terrain that was never authored.

---

## 3. Evaluation

### Fits Hypersteel?

**Yes, in slices, not as a planet pager.**

Campaign is overbuilt *directed* space. The win is:

- Japan / elevator / McMurdo vistas without 40k chair draws.  
- Pipe forests readable at range as tubes.  
- Interiors `QueueFree` when the door is sealed.  
- Bad PCs force Lod2 earlier.

The non-win is DH horizons of unauthored desert. Don’t.

### What is actually expensive in *this* game

Not “a billion tris.” It is:

- Lights + shadows in instanced interiors  
- Jolt bodies on clutter  
- Unique materials  
- Scripts on sleeping hydras  

So unloading **Gameplay** (scripts, lights, convexes) is the first 80%. LodMesh/atlas is the remaining 20% for the *view* from a ridge. If you only shipped Gameplay load/unload + Godot import LOD, you would already feel Chunx. Named collapse + atlas is for the pipe-city shots.

### Risks

| Risk | Why | Mitigation |
|---|---|---|
| Pop | Gameplay ↔ LodMesh silhouette mismatch | Overlap band; same trim texture; bake LodMesh from the *same* high mesh |
| Wallrun lie | Mid LOD hull ≠ high collision | Hero hull frozen; never mid-LOD a mountable without hull |
| Baker rot | Tags forgotten, atlas seals holes | Fail toward mesh; CI warning on untagged high-tri; grate-gap test |
| Thrash | Bhop across cell edge | Hysteresis; don’t page on velocity alone |
| Tick desync | Skipped gunner still “should have shot” | Important if LOS to player in last T seconds |
| Scope | Building Nanite | Cap: `@tool` baker + observer. No plugin fork until 3 cells work |

### Cost to build (honest)

- Observer + holder + hysteresis: small (days).  
- Threaded `ResourceLoader` + promote/demote: small.  
- Godot mesh LOD + visibility ranges on existing meshes: free-ish.  
- Tag schema + primitive emit (pipe/beam): medium.  
- Atlas flatten (raster cell from 2–4 views, pack): medium-hard.  
- Tick scheduler with skip-ahead: small if you only do it on NPCs you own.  
- Forking Chunx to understand tags: **do not**, until baker outputs are stable.

Chunx is a *consumer* of `Gameplay` / `LodMesh` / `LodAtlas`. If it cannot eat three products, write a 150-line observer instead of modifying the plugin.

### Alternatives (cheaper)

A. Chapter `ChangeScene` + 5–10 hand layers (pipeworks int, stadium). No grid.  
B. A + Godot per-mesh LOD + vis ranges.  
C. A + B + unload interior packeds by trigger (no atlas).  
D. Full system above.

Recommendation: **C first** on one McMurdo POI and one Japan exterior vista. If the ridge shot is still 8ms of chairs, then D (named collapse + atlas). If not, stop.

---

## 4. Prototype plan (still not Chunx)

1. Pick **one** cell (barracks exterior or pipe rack).  
2. Hand-make LodMesh (decimate) and one atlas strip for bolts. No automation.  
3. Observer: distance swap Gameplay ↔ LodMesh. Measure draw calls, tris, Jolt count.  
4. Add one `Pipe` collapsed by hand. Screenshot at 20 / 60 / 120m.  
5. Add skip-ahead on one idle drone in that cell.  
6. Only if (3)+(4) win ≥30% GPU or let a low preset exist: write the `@tool` baker.  
7. Only if baker works on 3 cells: wrap with Chunx *or* keep the observer.

Success bar: low preset plays the ridge at 60 on a 1660-class without looking like a painting she can land on.

Fail bar: pop obvious, wallrun desync, baker needs per-asset babysitting.

---

## 5. Decision

**Do not modify Chunx now.**

The last three prompts compose into a coherent system. It is not Nanite and should not try to be. It is **semantic HLOD + cell paging + skip-ahead ticks**.

Worth building if overbuilt vistas are a real frame-time line item. Not worth building as infrastructure before a second chapter exists in-engine.

Locked intent:

- Far = worse mesh + paint for hair, not a DH span.  
- Touch = real cell.  
- Thin compact / cheap % = atlas. Long thin = tube. Hero = hull forever.  
- Ticks skip unless they can hurt her.  
- Baker before pager. Pager before plugin fork.

## 6. Split: Chunx + lod_ge (paused)

Do not fork Chunx. It only pages packeds.

**Tool:** `lod_ge` — editor `@tool` only. Runtime never bakes.

### Tags (scene / MeshInstance / cell root)

Untagged = `Decimate`.

| Tag | Bake |
|---|---|
| `Hero` | Keep hull. Do not flatten. |
| `Pipe` | Prism along long axis / spline |
| `Beam` | Box |
| `Tank` | Cylinder / capsule |
| `Facade` | Quads + atlas normal |
| `Decimate` | QEM |
| `Pixel` | Force atlas |
| `Skip` | Ignore (vfx, volumes) |

Auto-pixel: untagged and (longest axis < 20cm **or** cheap % of selection). Long infra must be tagged `Pipe`, not left to auto.

### What it does

1. Select objects or cell root.  
2. Read tags.  
3. Write `lod/LodMesh`, `lod/LodAtlas`, `lod/LodHull` (hull if any Hero).  
4. **Swap preview** in editor: Gameplay ↔ LodMesh ↔ Atlas+Hull.  
5. Rebake on dirty source.

Chunx near = original scene. Far = `lod/` products.

### Paused

Come back when a vista actually costs GPU. Until then: chapter cuts + interior unload + import LOD. No extra tags before one hand-baked cell.
