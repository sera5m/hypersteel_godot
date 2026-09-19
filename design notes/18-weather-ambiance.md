# Weather and ambiance

Weather is its own system: planet row + local volumes + baked wind map.
It does **not** live inside StatusComponent. WeatherManager samples the field and hands tag *names* off. Status applies them. Weather does not Hurt() and does not own HP.

GPU particles / audio in a player bubble. Do not rain on the whole Athena.

## Stack

1. Planet preset
2. Mission row (normal / abnormal / rare)
3. Local volumes
4. Audio bed
5. Wind: **bake-time ray-trace** into `WindPropagationMap`. Runtime lookup. Live raycast opt-in when geo changes.

## Wind bake

- Cast prevailing wind through cells / corridor segments at level cook.
- Each bounce drops one `WindTier`. Sealed cells `occluded`.
- Dijkstra / A* on the segment graph fills halls when a door opens — do not recast the map.
- Code: `Scripts/WeatherManager/`

Tiers: Still < Breeze < Wind < Gale < Storm. Storm launches physics. Hulls ignore breeze.

## Rows write existing tags

Rain/Downpour = Wet. Snow = Cold. Heat = Hot. Steam = Wet+Hot. Smoke/Dust = Sanded. Smog/Acid/Spores = Toxic. Aurora/EclipseAthena = Zapped. Explosive gas = ExplosiveCoated.

Luna = vacuum. Suit audio only. Sun-side Hot / shadow Cold.

## Not

Map-wide particle sim. Second heat meter. Runtime RT every tick. Ion storm that disables the gun in your hands.
