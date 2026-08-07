# Progress & Next Steps

Status snapshot for picking this project back up without re-deriving context.
**Update this file at the end of a work session** so it stays trustworthy — treat it
as the first thing to read (and the last thing to update) in any future session.

Last updated: 2026-08-07. Everything below is pushed to `origin/main`, latest commit
`8aa52cd`.

**Repo location:** `C:\Users\marti\repo\food-be` (moved from `D:\repo\food-be` at some
point — a stale, untracked copy of `docs\` may still exist at the old `D:\` path;
ignore it, the `C:\` one is canonical).

## What exists right now

**Repo setup** — `.gitignore`, `global.json` (.NET 10 SDK pinned), `Directory.Build.props`
(nullable, analyzers, warnings-as-errors in Release), `.editorconfig` (naming/style,
mostly `warning` severity; EF Core migration files are exempted via `generated_code =
true`), `CLAUDE.md` (working agreements + build/test commands).

**Docs** (all in `docs/`):
- `architecture.md` — Clean Architecture, 5 projects. PostgreSQL, MediatR, NUnit,
  FluentValidation decided. Auth approach and hosting target still genuinely
  undecided.
- `business-description.md` — full business rules (onboarding/macro targets, meal
  logging, recipes, planner, activity logging). A few open questions remain at the
  bottom (exact macro-split ratios, ingredient tag moderation).
- `database-design.md` — ER diagram + schema. Primary keys are `bigint identity`
  (decided; means per-user authorization checks in `Food.Api` must be solid since IDs
  are enumerable).
- `deployment.md` — netcup VPS 500 G12 setup plan/progress (see "Infrastructure /
  deployment" below).

**Solution**: `Food.slnx`, five projects under `src/`, plus `tests/`.

- **`Food.Domain`** — fully implemented. Entities/value objects for Users, Nutrition
  (incl. `MacroTargetCalculator`: Mifflin-St Jeor BMR → TDEE → goal-adjusted calories →
  macro split — ratios are placeholders, not confirmed), Ingredients, Recipes,
  Logging/Planning (`LoggableItem` — the shared "recipe portion or raw ingredient,
  exactly one" value object used by `MealLog`/`FavoriteMeal`/`PlannedMeal`), Activities.
  **`Food.Domain.Tests`**: 32 tests, all passing.

- **`Food.Application`** — two complete vertical slices:
  - **Create ingredient** (`Ingredients/CreateIngredient/`).
  - **User & profile + macro target** (`Users/`): `CreateUser`, `SetUserProfile`
    (creates/updates the profile, then auto-recalculates and persists a new
    `NutritionTarget` via the already-tested `MacroTargetCalculator`),
    `GetCurrentNutritionTarget`, `SetManualNutritionTarget` (user overrides the
    calculated target directly — just another versioned row with
    `IsManualOverride=true`, no special-casing needed in the read path).
  Plumbing in place: MediatR 14.x + `ValidationBehavior` pipeline + FluentValidation,
  wired via `DependencyInjection.AddApplication()`. Ports: `IClock`, `IUnitOfWork`,
  `IIngredientRepository`, `IUserRepository`, `IUserProfileRepository`,
  `INutritionTargetRepository` — all implemented (see Infrastructure below).
  **`Food.Application.Tests`**: 13 tests (Ingredients slice only so far; Users slice
  has no unit tests yet).
  **Note on MediatR licensing**: v13+ (we're on 14.2.0) is dual RPL-1.5/commercial,
  not MIT. Decision made: stay on it, acceptable for a solo/small project under the
  free Community tier. Revisit if this ever becomes a commercial product at scale.
  **No real auth**: `UserId`/`CreatedByUserId` are just whatever the request body
  says — deliberately deferred (see "Next step" below), but now load-bearing across
  two slices.

- **`Food.Infrastructure`** — `FoodDbContext` + configurations for `Ingredient`,
  `User`, `UserProfile`, `NutritionTarget`. `MacroBreakdown` mapped as an EF Core owned
  type everywhere it appears; enums (`Sex`, `ActivityLevel`, `Goal`) stored as
  **strings**, not raw ints (avoids silent corruption if enum order ever changes).
  **Known simplification**: `ingredient_tags` is mapped per-ingredient, not as the
  shared/normalized many-to-many `database-design.md` originally envisioned — the
  Domain's `IngredientTag` is a value object with no identity, so true cross-ingredient
  sharing would require making it an entity first. Deferred; matches the doc's existing
  open question about whether tags should be fixed/seeded or user-extensible.
  Local `dotnet-ef` tool pinned via manifest (`dotnet-tools.json` at repo root, not
  `.config/` — non-standard location but confirmed working). Two migrations applied
  (`InitialCreate`, `AddUsersAndNutritionTargets`) to the local `food_dev` database.
  `MealLog`/`FavoriteMeal`/`PlannedMeal`/`LoggableItem` are **not** mapped yet — that's
  a harder problem (the owned value object references *other entities* conditionally)
  and nothing needs it yet.

- **`Food.Api`** — wired: `AddApplication()` + `AddInfrastructure()` in `Program.cs`.
  Endpoints: `POST /api/v1/ingredients`; `POST /api/v1/users`,
  `PUT /api/v1/users/{userId}/profile`, `GET /api/v1/users/{userId}/nutrition-target`.
  JSON enums serialize as strings (`JsonStringEnumConverter`), matching DB storage.
  **Scalar** interactive API docs wired at `/scalar` in Development (`.claude/launch.json`
  lets the `food-api` config be previewed via the Browser tooling). No auth yet.
  Verified end-to-end against live local Postgres for both slices.

All 45 tests pass; solution builds clean (0 warnings/errors) in Debug and Release.

## Infrastructure / deployment (see `docs/deployment.md` for full detail)

- **Local dev**: PostgreSQL 18 installed natively on Windows (not a container — this
  machine's CPU virtualization is disabled in BIOS and not accessible to enable, which
  rules out WSL2/Podman Desktop for local work). Role `martin` / database `food_dev`,
  connection string stored in **.NET User Secrets** (not in git). Deliberately matches
  the VPS `development` credentials for simplicity.
- **VPS** (netcup 500 G12, Debian 13, IP `159.195.215.80`): Podman + podman-compose
  installed (chosen over Docker for rootless containers — no privileged daemon).
  `/opt/food/production/` and `/opt/food/development/` directory layout created.
  `development`'s Postgres container is **running** (bound to `127.0.0.1` only).
  `production` deliberately not stood up yet — nothing to deploy there.
  **Still open**: SSH hardening (key-only root login — a key is installed but
  password auth hasn't been disabled yet), firewall (ufw) configuration, and the
  scoped `deploy` user for future CI/CD (not needed until CI/CD is actually wired up).

## Next step

Business-logic roadmap (from `business-description.md`), roughly in dependency order —
decided to keep deferring auth (see below) and build these next, one slice at a time:

- [x] Ingredients: create
- [ ] Ingredients: list/search, get by id
- [x] Users & profile: create user, set profile + auto-calculate target, get target
- [x] Users & profile: manual target override
- [ ] Recipes: create (ingredients + quantities), get (computed macros), list
- [ ] Meal logging: log a meal (raw ingredient or recipe portion), get a day's logs,
      **daily dashboard query** (macros vs. target) — needs the harder
      `LoggableItem`/`MealLog` EF mapping deferred during Infrastructure work
- [ ] Favorites: save/list/remove a favorite meal
- [ ] Activity logging: log activity, adjust the day's calorie target
- [ ] Weekly planner: plan a meal for a future date, check planned macros vs. target
      (same `LoggableItem` mapping problem as meal logging)
- [ ] Shopping list: aggregate ingredients across planned meals in a date range

Also open, not blocking:
- **VPS hardening** — finish SSH (`PermitRootLogin prohibit-password`,
  `PasswordAuthentication no`) and firewall (ufw), left open while getting Postgres
  running. Worth doing before this server handles anything real.
- **Auth** — deliberately still deferred (decided 2026-08-07) despite almost every
  slice above being per-user data. Revisit as its own dedicated step once more
  business logic exists to protect, per the discussion in this session.

## Other open decisions (not blocking, but unresolved)

- Auth approach — ASP.NET Core Identity + JWT is proposed in `architecture.md`, not
  confirmed.
- Hosting/deployment target for `production` — VPS infra exists but nothing decided
  about when/how to actually stand up a production environment.
- Exact macro-split ratios in `MacroTargetCalculator` — implemented with placeholder
  constants, flagged in code and in `business-description.md`.
- Whether `ingredient_tags` should be fixed/seeded vs. fully user-extensible (also now
  tied to whether `IngredientTag` needs to become an entity — see Infrastructure note
  above).

## Working agreement reminder

Small steps, confirm before `git commit`/`git push`, ask when something's a judgment
call rather than assuming. See `CLAUDE.md` for the full list.
