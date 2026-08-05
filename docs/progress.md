# Progress & Next Steps

Status snapshot for picking this project back up without re-deriving context.
**Update this file at the end of a work session** so it stays trustworthy — treat it
as the first thing to read (and the last thing to update) in any future session.

Last updated: 2026-08-05. Everything below is pushed to `origin/main`, latest commit
`b547e26`.

## What exists right now

**Repo setup** — `.gitignore`, `global.json` (.NET 10 SDK pinned), `Directory.Build.props`
(nullable, analyzers, warnings-as-errors in Release), `.editorconfig` (naming/style,
mostly `warning` severity), `CLAUDE.md` (working agreements + build/test commands).

**Docs** (all in `docs/`):
- `architecture.md` — Clean Architecture, 5 projects. PostgreSQL, MediatR, NUnit
  decided. Validation library is FluentValidation (now implemented, effectively
  decided even though the doc's checklist may still show it as open — see below).
  Auth approach and hosting target are still genuinely undecided.
- `business-description.md` — full business rules (onboarding/macro targets, meal
  logging, recipes, planner, activity logging). A few open questions remain at the
  bottom (exact macro-split ratios, ingredient tag moderation).
- `database-design.md` — ER diagram + schema. Primary keys are `bigint identity`
  (decided; means per-user authorization checks in `Food.Api` must be solid since IDs
  are enumerable).

**Solution**: `Food.slnx`, five projects under `src/`, plus `tests/`.

- **`Food.Domain`** — fully implemented. Entities/value objects for Users, Nutrition
  (incl. `MacroTargetCalculator`: Mifflin-St Jeor BMR → TDEE → goal-adjusted calories →
  macro split — ratios are placeholders, not confirmed), Ingredients, Recipes,
  Logging/Planning (`LoggableItem` — the shared "recipe portion or raw ingredient,
  exactly one" value object used by `MealLog`/`FavoriteMeal`/`PlannedMeal`), Activities.
  **`Food.Domain.Tests`**: 32 tests, all passing.

- **`Food.Application`** — one complete vertical slice: **create ingredient**
  (`Ingredients/CreateIngredient/`: command, handler, FluentValidation validator).
  Plumbing in place: MediatR 14.x + `ValidationBehavior` pipeline + FluentValidation,
  wired via `DependencyInjection.AddApplication()`. Ports defined for Infrastructure to
  implement later: `IClock`, `IUnitOfWork`, `IIngredientRepository`.
  **`Food.Application.Tests`**: 13 tests (handler + validator), using hand-written
  fakes for the ports — no DB involved.
  **Note on MediatR licensing**: v13+ (we're on 14.2.0) is dual RPL-1.5/commercial,
  not MIT. Decision made: stay on it, acceptable for a solo/small project under the
  free Community tier. Revisit if this ever becomes a commercial product at scale.

- **`Food.Infrastructure`** — empty scaffold. Nothing implemented yet.

- **`Food.Api`** — empty scaffold (template default minus the WeatherForecast sample).
  No real endpoints, no DI wiring for Application/Infrastructure yet.

All 45 tests pass; solution builds clean (0 warnings/errors) in Debug and Release.

## Next step (blocked on DB/VPS, expected ready 2026-08-06)

Build out `Food.Infrastructure` so the existing CreateIngredient slice runs end to end:

1. EF Core `DbContext` + entity type configurations mapping `Food.Domain` entities to
   the schema in `docs/database-design.md` (PostgreSQL via Npgsql).
2. Implement `IIngredientRepository` (EF Core-backed) and `IUnitOfWork` (thin wrapper
   over `DbContext.SaveChangesAsync()`).
3. Implement `IClock` (e.g. a `SystemClock` returning `DateTimeOffset.UtcNow`).
4. Wire `Food.Api`: call `AddApplication()` + a new `AddInfrastructure()`, add the
   first real endpoint (`POST /api/v1/ingredients`) so the slice is provably working
   over HTTP against a real database.
5. Run the first EF Core migration against the real Postgres instance once it exists.

## Other open decisions (not blocking, but unresolved)

- Auth approach — ASP.NET Core Identity + JWT is proposed in `architecture.md`, not
  confirmed.
- Hosting/deployment target — not discussed at all yet.
- Exact macro-split ratios in `MacroTargetCalculator` — implemented with placeholder
  constants, flagged in code and in `business-description.md`.
- Whether `ingredient_tags` should be fixed/seeded vs. fully user-extensible.

## Working agreement reminder

Small steps, confirm before `git commit`/`git push`, ask when something's a judgment
call rather than assuming. See `CLAUDE.md` for the full list.
