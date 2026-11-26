# Post shaders and screen effects

All post effects are toggleable. Defaults favor clarity over movie-camera simulation.

Hypersteel is operators with fear/menace, not a film. Baked lighting already carries the look (Signalis read / 2077 build). Screen effects are for style or specific game moments, not constant "cinematic" sludge.

Existing custom screen shaders live in `Shaders/` (chromatic, custom motion blur, lens distortion, radial blur, vignette, lens flare). Compositor stack is under `Compositor/` for heavier passes.

## Intended main effects

| Effect | Intent | Knobs | Notes |
|---|---|---|---|
| **Edge highlight** | Anime-style outline on characters / important silhouettes | Thickness 0–16 px. Color optional. Enable toggle. | Depth/normal edge detection in compositor, or inverted hull on characters. Keep center clean for aiming. |
| **Baked specular + raytracing** | Metal/plate read. Specular from maps + reflection probes. Optional live RT / SSR for hero surfaces. | Probe strength, RT toggle (off by default), roughness influence. | Baked is the hard carry per art direction. Runtime RT is opt-in only; do not force it. |
| **Color dots / halftone** | Overlay stylization (comic / poster dots). Inspired by Unreal Color Dots post material. | Dot size, screen angle, threshold, saturation, softness, input gain/bias. Enable toggle. | Screen-space only. Not for competitive clarity. Default off. |

## Custom / existing

Motion blur and the other `Shaders/` passes are already custom. They stay available but are not the default look. Motion blur in particular is one of the effects players hate most; keep it off by default and strength-limited if used for speed moments only.

## Avoid: typical movie camera nonsense

Players consistently rank these as the worst or most annoying post effects (ResetEra poll ~1.2k votes: chromatic aberration ~47%, motion blur ~17%, vignette ~11%, film grain ~8%, bloom ~12%; PC Gamer knockout tournament crowned motion blur the most hated). Common complaints: eye strain, nausea, lost detail, "stop covering the image," forced cinematic pretension that fights gameplay.

**Prefer to avoid or default off:**

- Full-screen motion blur (especially camera-driven). Per-object speed blur only if it serves a specific verb and is weak.
- Chromatic aberration (color fringing). Only if extremely subtle and toggleable; default off.
- Heavy film grain.
- Strong vignette that darkens edges.
- Aggressive depth of field that softens mid-ground targets.
- Constant lens flare / bloom that washes information.
- Lens distortion as a permanent filter.

These can exist as optional toggles for people who want them, but they must not be the shipped default. Clarity and readable silhouettes win. Edge highlight and dots are the deliberate stylizations; the rest of the "camera simulation" pile is not the identity.

## Implementation notes

- Compositor effects for the new full-screen ones (edge, dots). Character outlines can also be material-side if cheaper.
- Settings resource or compositor enable flags. Thickness / strength exposed.
- No forced cinematic stack. If an effect is on, the player turned it on.
