# Deployment

Status: **in progress** — being worked through step by step. Update the checklist below
as steps complete.

## Server

- Provider: netcup, VPS 500 G12.
- IPv4: `159.195.215.80`
- IPv6: `2a0a:4cc0:c4:4a7:8d3:81ff:fe2d:95`
- Hostname: `v2202608392879495089.quicksrv.de`
- OS: **Debian GNU/Linux 13** ("Trixie"), confirmed via the netcup VNC console.
- No domain name yet — real TLS (Let's Encrypt) is blocked on that; HTTP-only or a
  self-signed cert until a domain exists.

## Approach

- **Podman + podman-compose** (Compose-file-compatible) for Postgres, the API, and the
  web frontend, rather than native `apt`-installed services. Chosen over Docker
  specifically because Podman is daemonless and supports true **rootless** containers —
  no privileged background daemon, and no "add user to a group that's effectively
  root" gotcha the way Docker's `docker` group has. Config still lives as versioned
  compose files in this repo instead of undocumented manual commands.
- **Two fully separate environments on the same VPS**: `production` and
  `development`. Each gets its own Postgres container + named volume, its own Podman
  network, its own compose project name, and (for now) its own ports — so dev can
  never touch prod's data. Both run from built images; `development` tracks the
  `develop` branch, `production` tracks `main`/release tags.
- **Day-to-day coding happens locally**, not on the VPS. The VPS's `development`
  environment is a deployed/staging copy — updated by pulling the `develop` branch and
  rebuilding — used to test changes in a server-like setting before they reach
  production. It is not a live-edit environment.
- **Root for interactive admin, no sudo user** — a deliberate choice for manual work
  over the more conventional non-root-user setup. To still get real security benefit,
  SSH is hardened to **key-only root login** (`PermitRootLogin prohibit-password`)
  rather than leaving root password-authenticable over the internet indefinitely.
- **Separate scoped `deploy` user for CI/CD** — no sudo, runs rootless Podman only.
  GitHub Actions (or any automated deploy) uses this account's own SSH key, kept
  separate from the personal root key. Rationale: if a CI secret ever leaks, the blast
  radius is "can manage this account's containers," not "owns the machine."
- Commands are written here for the user to run manually via their own SSH session —
  not executed by Claude directly against the live server.

## Checklist

- [x] 1. Confirm OS (Debian 13), confirm root SSH access works
- [ ] 2. Install root's SSH public key, harden SSH (`PermitRootLogin prohibit-password`,
      `PasswordAuthentication no`)
- [ ] 3. Configure firewall (ufw): allow SSH/80/443 only (plus dev ports for now)
- [x] 4. Install Podman + podman-compose (podman 5.4.2, podman-compose 1.3.0)
- [x] 5. Create `/opt/food/production/` and `/opt/food/development/` layout
- [ ] 6. Create scoped `deploy` user (rootless Podman, no sudo) with its own SSH key
      pair, reserved for CI/CD (GitHub Actions) — production only
- [x] 7. `docker-compose.yml` (Podman-compatible) for PostgreSQL — **development**
      running (`food-dev-postgres`, Postgres 16, persistent volume, bound to
      `127.0.0.1` only). **Production deferred** — no reason to stand it up yet.
- [ ] 8. Add the API + web frontend containers per environment once they have real
      code to run; reverse proxy + real TLS once a domain exists
- [ ] 9. Basic Postgres backup strategy (cron + `pg_dump`, or volume snapshot) —
      production
- [ ] 10. Optional: fail2ban for SSH brute-force protection

## Deferred / not urgent right now

- Step 2 (SSH hardening) and step 3 (firewall) are still open — got deprioritized
  while getting Postgres running. Worth circling back to before this server handles
  anything real.
- Step 6 (scoped `deploy` user) isn't needed until CI/CD is actually being wired up.
- Local Postgres for local development: deliberately not set up yet — nothing in the
  codebase needs it until `Food.Infrastructure` exists. Recommended approach when it's
  time: Docker Desktop (or Podman Desktop) on Windows running a local container, same
  pattern as the VPS, not a native Windows install.
