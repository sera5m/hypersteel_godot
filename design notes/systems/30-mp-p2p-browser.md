# Multiplayer — P2P + Tailscale master + server browser

2026-11-26. Design only. Not started. No AWS.

## Shape

- **Listen-server P2P.** Host is a player (or a headless process the host started). Clients connect directly. No dedicated process required in v1.
- **Tailscale master** for discovery and matchmaking only. It does not relay packets. It lists servers, hands out Tailscale addresses, and dies if the host leaves.
- **No cloud bill.** Host runs the simulation. Clients run prediction + rollback on movement/aim. Damage is host-authoritative.
- **Classic CS browser.** One table. Hover a row → description panel on the opposite side of the screen.

## Browser columns

| Column | What it shows |
|---|---|
| Server name | Host-set string. Truncated. |
| Gamemode | Short name from the GameMode resource (FFA, TDM, Attrition, etc.). |
| Players | `current / max`. |
| Time | Time left in the current round / match. |
| Score | Current score / score limit. |
| Other | Compact flags (password, mods, latency, map, permanent, dev). |

Hovering a row opens a side panel:

- Top half: gamemode attributes (score type, time limit, team count, win condition, any special rules from the GameMode def).
- Bottom half: free-text description the host typed. Can be empty. No markup.

JSON shape the master returns (example):

```json
{
  "servers": [
    {
      "id": "ts-xxxx",
      "name": "Ryko’s loft",
      "gamemode": "TDM",
      "players": 6,
      "max_players": 12,
      "time_left_s": 420,
      "score": [18, 12],
      "score_limit": 50,
      "map": "solar_farm",
      "password": false,
      "permanent": false,
      "dev_hosted": false,
      "description": "no camping the beam",
      "tailscale_addr": "100.x.x.x:7777",
      "host_name": "sera5m"
    }
  ]
}
```

## Tailscale master duties

- Hosts register on start (name, gamemode def id, max players, map, description, Tailscale IP:port, permanent flag, dev flag).
- Clients query the list (filtered by gamemode, map, password, open slots, permanent, dev).
- Master stores nothing persistent. Crash = empty list. Hosts re-register every 30 s.
- No relay. Once a client has the Tailscale address it connects P2P.
- Optional: host can set a short password. Master never sees the password; client sends it on the P2P handshake.
- Permanent / dev servers still re-register; the master just marks the flag. If the process dies the listing disappears until it comes back.

## GameMode resource

Data-driven. Composed of rules, not a class tree.

- Spawn rules (where, when, team assignment).
- Team rules (none, fixed, dynamic).
- Score rules (kills, points, time, objective).
- Population rules (min to start, max, bot fill later).
- Win / end conditions.

Examples already sketched: FFA, TDM, Attrition (TF2-style points). New modes are new Resources, not new code.

## Server / world persistence

Three lifetime tiers, plus two orthogonal attributes.

| Tier | Lifetime | Who runs it | What persists |
|---|---|---|---|
| **Match** | Until host leaves or score/time ends | A player | Nothing. Loadout is client-side. |
| **Session** | Host keeps the process open | A player or a small always-on box | Scoreboard, current map state, optional admin list. Dies with the process. |
| **Permanent / dev** | Intended to stay up | Host’s machine or a cheap always-on box the host owns | Optional world state (placed objects, unlocked doors, custom loadouts). Still no central DB. |

**Orthogonal attributes** (can be set on any tier):

- **Permadeath open world** — Minecraft-hardcore style. Limited or no respawns. Multiplayer mercs. Death is expensive; you may buy a limited number of reprints or lose the character. Does not conflict with permanent.
- **Perma-world** — the world state is meant to stick around. Respawns can be infinite, limited per match, limited per character, or none. Permanent worlds can have infinite respawns. Permanent is just “the process is expected to stay up”; perma-world is “the map/state is meant to accumulate.”

Permanent and dev servers set the `permanent` / `dev_hosted` flags so the browser can filter or badge them. Permadeath and perma-world are additional flags on the GameMode or server listing.

Dev-hosted can be marked by the Tailscale account or a simple shared secret the master checks. Not a paid tier.

No central player database. A permanent server can optionally write a local JSON/SQLite file the host controls (unlocked doors, custom rules, ban list, per-player stats for that server). Clients never talk to that file directly.

## Player data

- **Loadout** is client-side. You bring your hip, pack, perks, and guns. The server validates the sheet against the GameMode’s allowed list (or ignores it if the mode is “bring whatever”).
- **Identity** is Tailscale identity for the session. No global accounts. Ban list is per-server and local to the host.
- **Stats are per-server.** Kills, deaths, score, time played, reprints used, etc. live on the host for that server. One instance of the stat blob per player per server. Not shared across servers.
- **On leave:** the server writes a hidden encrypted file on the client’s hard drive containing that player’s stats for this server. Client cannot edit it in a useful way (encryption + integrity check). Next time they join the same server the client presents the file; the server can accept it, ignore it, or reconcile with its own copy.
- **Progression** stays local to the server. No ranks or unlocks on the master. Permanent servers keep their own copy; the encrypted client file is just a portable receipt so the player doesn’t lose their numbers if the host’s process restarts.
- Match-only servers still track stats for the duration of the match and can write the encrypted file on disconnect if the host wants. They just have nothing to reconcile against later.

## What is not in v1

- Dedicated servers that are not also a listen-server process.
- Relay fallback if Tailscale is blocked.
- Persistent global accounts or ranks.
- Cross-platform (Windows / Linux listen-server only).
- Anti-cheat beyond “host is the authority.”
- Central world database.

## Notes

- Host migration is out of scope. Host leaves → match ends (unless it is a permanent process that restarts itself).
- Latency display is a simple ping to the Tailscale address before join.
- Description panel is pure UI. No rich text, no links, no images.
- Permanent servers are still P2P under the hood; the “master” is only the phone book.

## Locked decisions (2026-11-26)

- **Receipt reconcile:** both ways. Merge non-conflicting fields; host wins on conflicts. Client encrypted file is a backup/receipt, not the sole source.
- **Browser filters (v1):** full CS-style. Gamemode, map, password, permanent, dev, latency range, empty/full toggle.
- **Permanent / host process:** host’s own machine. Scripting extensions allowed so the host can restart the process, set gamemode, and other admin actions (scratch-style or simple script hooks). Still their hardware; master only sees the re-registration.
- **Bots:** depends on GameMode. Host can flip a toggle to fill with bot-operated multiplayer mercs. Bots use the same AI as campaign soldiers. Not forced on every mode.


## Locked decisions (2026-11-26, round 2)

- **Master runtime:** C# / Godot headless. Same language as the game.
- **Passwords:** none in v1. Open servers only.
- **Latency:** both. Client pings the Tailscale address when possible; fall back to master heartbeat age.
- **Host death:** script auto-restarts the process. Clients can rejoin the same map/GameMode. Stats come from the encrypted receipt if the host’s in-memory copy is gone.
- **Host scripting (v1):** config file + restart flag only. No code hooks yet.
- **Ban list:** host local file only. Dies with the machine.
- **Map filter:** both. Known maps get pretty names from local defs; custom maps show the raw string the host sent.
- **Dev badge:** none in v1. Permanent flag only.
- **Max players:** host sets it; master enforces a hard cap (exact number still open).
- **Browser UX while waiting:** spinner + last-known cache + direct-connect box always visible.
- **Chat:** text only, host-relayed, in v1. Positional voice later.

## Networking transport (2026-11-27)

**Preferred: Steam Networking.**  
Godot has a Steamworks/GodotSteam path. Steam handles NAT traversal, hole-punching, and an optional relay. Players already have Steam if the game ships there. No extra install. Master still only does discovery (list + Steam lobby/ID handoff). Actual packets go Steam P2P.

**Fallback options if Steam is not acceptable:**

- **Tailscale bundled.** Ship the Tailscale client (or a minimal userspace WireGuard) as a prerequisite/installer step. Still an extra binary and account, but the player does not have to find it themselves.
- **Plain UDP hole-punching + optional cheap relay.** Master coordinates the punch. Relay only when punch fails. Relay can be a single cheap VPS the host runs, or skipped entirely for “LAN / same-network only” in early tests.
- **Accept Tailscale as a hard prereq.** Document it. Simplest code, highest friction.

**Current lean:** Steam Networking first. Tailscale remains the documented alternative if Steam is off the table. Do not start listen-server packet code until the transport is locked.

