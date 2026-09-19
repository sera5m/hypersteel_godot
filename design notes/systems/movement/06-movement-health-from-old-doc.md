# Movement + health goals (mined from old Google Doc)

Source: out-of-date design dump. Plot in that doc is superseded by `01-campaign-spine.md`. Kit numbers below were playtested in Unreal. Use as **goals**, not as a second bible.

## What to keep (proven / on-theme)

### Movement contract
World is a tac-FPS. Ryko is not. Bhop / source air / slide-ragdoll are **canon** (except cattle-player nonsense).

From the tested jumpkit:

- Wall kick on jump (up + off)
- Source-like air (needs downtune; old note: too fast)
- Crouch: huge knockback resist
- Slide: from move+crouch; wall-bounce allowed; high-speed slide **ragdolls** extras; faster downhill
- Mild ramp-slide
- Wallrun: faster move, slower thrust regen
- Midair jump: 1 thrust
- Air dash: 1.5 thrust, WASD-relative
- Regen: +2 on land if below 2; passive ~0.5/s; ~0.28/s on wallrun (values were already marked “edit”)
- Later patch: land-speed refunds stamina (8 / 24 / 50 m/s → 25 / 75 / 150%)
- After dash: melee/hurtboxes swell (slam into people)
- Slam on surface = small stun shockwave
- Speed cap relaxes during air dash

**Goal:** Titanfall kit + Quake air, one shared thrust/stamina bar, velocity is a damage stat.

### Melee / grab (left hand)
- Punch scales with velocity; crit small enemies delete; big ones → glory
- Sword: slash + **continuous parry** that starts on first projectile and drains shared bar (tested: single parry was bad vs burst)
- Kick: jump-look-at-enemy; midair kick throws if already holding
- Grab: no damage; heavier target → you cling to *them*; can steal weapon/shield with yanks
- Surface grab: hang, shoot, stamina regen, jump off
- Throw: impact damage from speed; heavier than you → you pop off, they don’t ragdoll

**Goal:** left hand is the pile-driver / grab. Right hand is guns. Grapple kit as a *pickup* in one mission is fine; default cling is body-physics, not a Spider-Man button.

### Heal
Old text already matches current fiction:

- Small reserve mass → passive heal to a cap
- Past cap: need material
- Damaged enemies drip mass (Ultrakill bleed)
- Corpses = pickup, magnetize in range
- Melee kill = big heal + ammo
- Glory / finisher = huge instant heal + ammo

**Goal:** kill-to-heal is the loop. Medkits exist as crumbs, not the identity.

### Limb / locational (reconcile with current rule)
Old doc: Head / Chest / R-Arm / L-Arm / Legs, tiers 0–3, glory or mass drops a tier. Very readable (hit → problem → crisis).

Current rule (keep): first injury on a limb **matters**; further hits on that wreck glance more unless dead-center, which **penetrates**. She is redundant. Not a human limp-sim.

**Merge goal:**

- Keep 3 tiers per part for *feedback* (audio/visor/sway)
- Do **not** let Tier 3 legs turn this into a 50% walk simulator for long. Crisis is real but siphon/glory is the fix, not a crawl chapter
- Head T3 HUD-fail is allowed as a short crisis (fits chip-brain)
- Chest T3 can tax stamina/heal efficiency (reactor / core)
- R-arm T3: jam + recoil bites you (weapon platform)
- L-arm T3: weak melee/nades until glory (pile-driver hurt)
- After a part is already injured: apply the glance/penetrate rule so dumping into hamburger is stupid

### Guns (Eternal seasoning)
Keep the *pattern*: start rifle, pickup alts, distinct primary/alt.

| Old gun | Keep? |
|---|---|
| Smart rifle (lock stages, burst 6 @ 2k rpm, 0.4s pause, auto-fire weak points) | Yes as starter. Lock is “she has targeting.” Recalibrate-assist after 100–200 misses is the same system going parental |
| Quad thermite shotgun (Pyro drop) | Yes. Recoil as thrust. Broken-arm 10% extra limb hit |
| Charge plasma (pre-Pyro pickup) | Yes. Magnetic spike alt. Overheat. Vision noise |
| Coilgun stages 0–4 (Valkarie midgame) | Yes as *her* language / stolen rail cousin. Ubercharge eating the gun is optional spice |
| Hand laser cutter | Yes. Infinite, drains jump/ability. Cold bonus / hot penalty. Rebound. Face-blind. Holstered = +10% run |
| Micro-missile SMG (Tone-like) | Maybe. Easy to cut if loadout is fat |
| HE / ICE nades (shoulder pod Q) | ICE stays (thermal shock combo with General liquid-air). HE shootable in air |

Caliber sheet (14mm ≈ .50, 8mm ≈ 7.62, plasma = Li pellets) is world texture. Keep. Lithium-hydride typo → lithium hydride / pellets already in chassis lore.

### Player-as-lobe tells (already locked)
Taunt T. Empty-pocket pat. Ally veto `intrusive. discarded.` Miss-streak `recalibrating targeting.`

## What to cut or quarantine

- **Defcon / Harwood / Maria / Greenland nuke choice** — different game. Sidecar at best. Wink-adjacent QTE squad. Do not graft onto Ryko.
- **Second supercarrier + teleport-nuke as Ch0 closer** — you already marked lazy. Spine killed it.
- **“IS WHAT I WOULD SAY IF I WAS A LITTLE BITCH”** Enduring fake-out — cheese; test against commit rule. Bell is better than the meme.
- **Open-world ODST Dark Arm + lab moral choices** — replaced by power/server POIs.
- **UE5 version-chasing changelog** — history, not goals. Systems-first stays.
- Alt-history WW2/Germany novel — not needed for movement/health. Campaign is Sino-Japanese near-future; don’t reopen the enlightenment essay to tune a dash.

## Goal one-pager (ship this)

1. Shared thrust bar: jump, dash, wallrun tax, land refund.  
2. Slide + speed = weapon (ragdoll extras).  
3. Continuous parry on that same bar.  
4. Velocity punch / grab / throw.  
5. Heal = mass + corpses + glory.  
6. Limbs: 3-tier feedback, siphon resets, glance-unless-center after first break.  
7. Six-or-fewer distinct guns with alts; cutter is a tool that pauses the kit.  
8. Motor/executive desync stays scarce.

If a new idea does not serve one of those eight, it is the old doc talking.