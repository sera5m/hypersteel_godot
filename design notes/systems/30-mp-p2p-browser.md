# Multiplayer — P2P + Tailscale master + server browser

2026-11-26. Design only. Not started. No AWS.

## Shape

- **Listen-server P2P.** Host is a player. Clients connect directly. No dedicated process in v1.
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
| Other | Compact flags (password, mods, latency, map). |

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
      "description": "no camping the beam",
      "tailscale_addr": "100.x.x.x:7777",
      "host_name": "sera5m"
    }
  ]
}
```

## Tailscale master duties

- Hosts register on start (name, gamemode def id, max players, map, description, Tailscale IP:port).
- Clients query the list (filtered by gamemode, map, password, open slots).
- Master stores nothing persistent. Crash = empty list. Hosts re-register every 30 s.
- No relay. Once a client has the Tailscale address it connects P2P.
- Optional: host can set a short password. Master never sees the password; client sends it on the P2P handshake.

## GameMode resource

Data-driven. Composed of rules, not a class tree.

- Spawn rules (where, when, team assignment).
- Team rules (none, fixed, dynamic).
- Score rules (kills, points, time, objective).
- Population rules (min to start, max, bot fill later).
- Win / end conditions.

Examples already sketched: FFA, TDM, Attrition (TF2-style points). New modes are new Resources, not new code.

## What is not in v1

- Dedicated servers.
- Relay fallback if Tailscale is blocked.
- Persistent accounts or ranks.
- Cross-platform (Windows / Linux listen-server only).
- Anti-cheat beyond “host is the authority.”

## Notes

- Host migration is out of scope. Host leaves → match ends.
- Latency display is a simple ping to the Tailscale address before join.
- Description panel is pure UI. No rich text, no links, no images.
