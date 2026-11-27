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

Three tiers. All still listen-server; the difference is lifetime and who pays for the process.

| Tier | Lifetime | Who runs it | What persists |
|---|---|---|---|
| **Match** | Until host leaves or score/time ends | A player | Nothing. Loadout is client-side. |
| **Session** | Host keeps the process open | A player or a small always-on box | Scoreboard, current map state, optional admin list. Dies with the process. |
| **Permanent / dev** | Intended to stay up | Host’s machine or a cheap always-on box the host owns | Optional world state (placed objects, unlocked doors, custom loadouts). Still no central DB. |

- Permanent and dev servers set the `permanent` / `dev_hosted` flags so the browser can filter or badge them.
- Dev-hosted can be marked by the Tailscale account or a simple shared secret the master checks. Not a paid tier.
- No central player database. A permanent server can optionally write a local JSON/SQLite file the host controls (unlocked doors, custom rules, ban list). Clients never talk to that file directly.

## Player data

- **Loadout** is client-side. You bring your hip, pack, perks, and guns. The server validates the sheet against the GameMode’s allowed list (or ignores it if the mode is “bring whatever”).
- **Identity** is Tailscale identity for the session. No accounts. Ban list is per-server and local to the host.
- **Progression** is out of scope. No ranks, no unlocks stored on the master. If a permanent server wants unlocks it keeps them in its local file.
- **Stats** (kills, deaths, score this match) live on the host for the duration of the match and are sent to clients for the scoreboard. They vanish when the match ends unless the host is permanent and chooses to log them.

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
