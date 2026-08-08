# Progress & Next Steps

Status snapshot for picking this project back up without re-deriving context.
**Update this file at the end of a work session** so it stays trustworthy — treat it
as the first thing to read (and the last thing to update) in any future session.

Last updated: 2026-08-08. Committed locally as `f501c14` (Weekly planner + Shopping
list — **this completes the full business-logic roadmap** from
`business-description.md`), on top of Activity logging (`d9f0fdc`), Favorites
(`bfb911e`), and Meal logging/Dashboard (`72dc032`/`7b640b9`); not yet pushed to
`origin/main`.

**Repo location:** `C:\Users\marti\repo\food-be` (moved from `D:\repo\food-be` at some
point — a stale, untracked copy of `docs\` may still exist at the old `D:\` path;
ignore it, the `C:\` one is canonical).

**Permissions:** `.claude/settings.json` allows `Bash(*)` and `PowerShell(*)` plus the
Claude_Browser preview tools, so routine implementation work (build/test/run,
`curl`/`psql` checks, driving the local preview) no longer prompts. `git commit` and
`git push` are explicitly kept in the `ask` list — those still always need sign-off.
**Separately**, `curl`/`psql` calls were still individually re-prompting despite
`Bash(*)` — turned out to be a *sandbox network egress* check, a boundary independent
of the Bash tool permission. Fixed by adding `sandbox.network.allowedDomains:
["localhost", "127.0.0.1"]` to both `~/.claude/settings.json` (new, global — covers
every project) and this repo's `.claude/settings.json` (commit `c0cf8cb`). If a stray
individual-command allow-rule still shows up in `.claude/settings.local.json`, that's
a harmless leftover from before the fix, not a sign it's not working.

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
  **One change this session**: `LoggableItem` gained a private parameterless
  constructor purely for EF Core materialization — its existing 4-arg constructor
  can't be constructor-bound by EF because `Recipe`/`Ingredient`/`Quantity` are
  navigations, not scalar properties, so EF needs a no-arg path and sets the
  get-only properties' backing fields via reflection instead. Public API (the
  `FromRecipe`/`FromIngredient` factories) unchanged.
  **`Food.Domain.Tests`**: 32 tests, all passing.

- **`Food.Application`** — nine complete vertical slices:
  - **Ingredients** (`Ingredients/`): `CreateIngredient`, `ListIngredients` (search by
    name, filter by tag), `GetIngredientById`. `IngredientDto` flattens
    `MacroBreakdown`/`Tags` into a clean API shape rather than serializing Domain types
    directly.
  - **Users & profile + macro target** (`Users/`): `CreateUser`, `SetUserProfile`
    (creates/updates the profile, then auto-recalculates and persists a new
    `NutritionTarget` via the already-tested `MacroTargetCalculator`),
    `GetCurrentNutritionTarget`, `SetManualNutritionTarget`.
  - **Recipes** (`Recipes/`): `CreateRecipe` (loads each referenced `Ingredient` by id,
    builds the `Recipe` aggregate via `AddIngredient`, throws if an id doesn't exist),
    `GetRecipeById`, `ListRecipes`. `RecipeDto` includes both `TotalMacros` and
    `MacrosPerServing` (computed by the Domain, not stored).
  - **Logging** (`Logging/`): `LogMeal` (accepts either `RecipeId`+`ServingsCount` or
    `IngredientId`+`QuantityGrams`, validated as exactly-one via a `Must` rule on the
    whole command; loads the referenced `Recipe`/`Ingredient` and builds a
    `LoggableItem` through its `FromRecipe`/`FromIngredient` factories), `GetDayMealLogs`
    (all of a user's logs for one `LogDate`). `MealLogDto` flattens `Item` into nullable
    recipe/ingredient fields plus computed `Macros`.
  - **Dashboard** (`Dashboard/`): `GetDailyDashboard` — looks up the `NutritionTarget`
    effective for the requested date (returns `null`, mapped to 404, if the user has
    none yet), sums that day's `MealLog` macros, and returns target/consumed/remaining
    plus the day's meal list. `Remaining` is **not** a `MacroBreakdown` — that Domain
    type enforces non-negative components via `Guard.NonNegative`, but remaining must be
    able to go negative (over target), so it's a separate unconstrained
    `MacroRemainder` record local to the Application layer. **Now includes the
    activity-calorie adjustment**: sums that day's `ActivityLog.CaloriesBurned`, adds it
    to `Target.Calories` (only calories — protein/carbs/fat/fiber targets are
    unaffected, per `business-description.md`), and computes `Remaining` against the
    *adjusted* target. `ActivityCaloriesBurned` is also exposed on the DTO directly so
    a client can show the adjustment amount, not just its effect.
  - **Activities** (`Activities/`): `LogActivity` (activity type, duration, calories
    burned — all client-entered, no MET-formula estimation per the business doc),
    `GetDayActivityLogs`. Simplest slice so far — `ActivityLog` has no `LoggableItem`,
    just plain scalars, so no EF owned-type complexity on the Infrastructure side.
    `GetDayActivityLogs` wasn't explicitly named in the roadmap wording (only "log
    activity, adjust the day's calorie target" was), but was added to mirror
    `GetDayMealLogs` — a write-only log with no way to read it back isn't useful.
  - **Favorites** (`Favorites/`): `SaveFavoriteMeal` (same exactly-one
    `RecipeId`+`ServingsCount`/`IngredientId`+`QuantityGrams` shape and validation as
    `LogMeal`, plus a required `DisplayName`), `ListFavoriteMeals` (per user),
    `RemoveFavoriteMeal` — returns `bool` (not found *or* owned by a different user
    both come back `false` → 404 at the API layer; the ownership check is deliberate
    defense-in-depth given `docs/database-design.md`'s enumerable-bigint-id warning,
    even with no real auth yet). `FavoriteMealDto` flattens `Item` the same way as
    `MealLogDto`.
  - **Planning** (`Planning/`): `PlanMeal` (identical shape/validation to `LogMeal`,
    just `PlanDate` instead of `LogDate`), `GetDayPlannedMeals`, `GetPlannedDashboard`
    (planned macros vs. target for a date — same shape as `GetDailyDashboard` but
    **no** activity-calorie adjustment: the planner targets future days that have no
    logged activity yet, so it checks against the plain `NutritionTarget`). No
    remove/edit of a planned meal yet — wasn't part of the roadmap wording ("plan a
    meal ... check planned macros vs. target"), left as a follow-up if needed.
  - **ShoppingList** (`ShoppingList/`): `GetShoppingList` — aggregates ingredient
    quantities across all planned meals in a `[StartDate, EndDate]` range, grouped by
    ingredient. Pure read/query, no new table (per `database-design.md`'s "Deliberately
    not a table" note — a stored, checkable-off list is a different, deferred problem).
    Handles **both** planned-meal sources per `business-description.md` ("planned
    recipes *and* standalone ingredients"): a direct ingredient contributes its
    `QuantityGrams` as-is; a recipe contributes each `RecipeIngredient.Quantity.Grams`
    scaled by `ServingsCount / Recipe.Servings` — **not** a flat multiply by
    `ServingsCount`, since `RecipeIngredient.Quantity` is the amount for the *whole*
    recipe (all servings), the same convention `Recipe.MacrosPerServing()` relies on.
    Caught this during E2E verification (a 2-serving recipe was initially about to
    double-count) before considering the slice done.
  Plumbing in place: MediatR 14.x + `ValidationBehavior` pipeline + FluentValidation,
  wired via `DependencyInjection.AddApplication()`. Ports: `IClock`, `IUnitOfWork`,
  `IIngredientRepository`, `IRecipeRepository`, `IUserRepository`,
  `IUserProfileRepository`, `INutritionTargetRepository`, `IMealLogRepository`,
  `IFavoriteMealRepository`, `IActivityLogRepository`, `IPlannedMealRepository` — all
  implemented (see Infrastructure below).
  **`Food.Application.Tests`**: 13 tests (Ingredients `CreateIngredient` slice only;
  everything added since, including this session's Logging/Dashboard/Favorites/
  Activities/Planning/ShoppingList slices, has no unit tests yet — verified via manual
  end-to-end testing instead, matching the precedent set by the Recipes slice).
  **Note on MediatR licensing**: v13+ (we're on 14.2.0) is dual RPL-1.5/commercial,
  not MIT. Decision made: stay on it, acceptable for a solo/small project under the
  free Community tier. Revisit if this ever becomes a commercial product at scale.
  **No real auth**: `UserId`/`CreatedByUserId` are just whatever the request body
  says — deliberately deferred (see "Next step" below), but now load-bearing across
  all nine slices.

- **`Food.Infrastructure`** — `FoodDbContext` + configurations for `Ingredient`,
  `User`, `UserProfile`, `NutritionTarget`, `Recipe`, `RecipeIngredient`.
  `MacroBreakdown` mapped as an EF Core owned type everywhere it appears; enums
  (`Sex`, `ActivityLevel`, `Goal`) stored as **strings**, not raw ints (avoids silent
  corruption if enum order ever changes).
  **`RecipeIngredient` is a real entity, not owned** (unlike `IngredientTag`) — it has
  its own `Id`, a required shadow FK `RecipeId` back to `Recipe` (explicitly marked
  `.IsRequired()`; EF defaults shadow FKs from a one-sided `.WithOne()` to *nullable*,
  which was wrong here and caught during migration review, not left as a silent bug),
  and a real FK to `Ingredient` with `DeleteBehavior.Restrict` (don't cascade-delete
  recipes if an ingredient is ever removed).
  **Known simplification**: `ingredient_tags` is mapped per-ingredient, not as the
  shared/normalized many-to-many `database-design.md` originally envisioned — the
  Domain's `IngredientTag` is a value object with no identity, so true cross-ingredient
  sharing would require making it an entity first. Deferred; matches the doc's existing
  open question about whether tags should be fixed/seeded or user-extensible.
  Local `dotnet-ef` tool pinned via manifest (`dotnet-tools.json` at repo root, not
  `.config/` — non-standard location but confirmed working). Seven migrations applied
  (`InitialCreate`, `AddUsersAndNutritionTargets`, `AddRecipes`, `AddMealLogs`,
  `AddFavoriteMeals`, `AddActivityLogs`, `AddPlannedMeals`) to the local `food_dev`
  database.
  **`ActivityLog` mapped** — plain scalar entity (`ActivityLogConfiguration`), no owned
  types or FKs needed since it doesn't touch `LoggableItem`. Straightforward compared
  to `MealLog`/`FavoriteMeal`/`PlannedMeal`.
  **`MealLog`/`FavoriteMeal`/`PlannedMeal`/`LoggableItem` all mapped** — this was the
  deferred "harder problem" (an owned value object that conditionally references
  *other entities*), solved once for `MealLogConfiguration` and reused as-is (same
  shape, different table/columns) for `FavoriteMealConfiguration` and
  `PlannedMealConfiguration`: `OwnsOne(x => x.Item, ...)`, and *within* that owned-type
  builder, `HasOne(i => i.Recipe)`/`HasOne(i => i.Ingredient)` as ordinary (optional)
  entity relationships with shadow FK properties explicitly named/column-named
  `RecipeId`/`IngredientId` (EF's default would have prefixed them `Item_RecipeId`/
  `Item_IngredientId` since they live inside an owned type — overridden to match this
  codebase's existing unprefixed-FK convention from `recipe_ingredients`), plus a
  nested `OwnsOne(i => i.Quantity, ...)` (owned-type-inside-owned-type, supported since
  EF Core 5). A `CHECK` constraint (`CK_meal_logs_exactly_one_source` /
  `CK_favorite_meals_exactly_one_source` / `CK_planned_meals_exactly_one_source`)
  enforces exactly one of `RecipeId`/`IngredientId` is set in each table, matching
  `database-design.md`. Required the `LoggableItem` Domain change noted above.
  `Recipe`/`Ingredient` FKs use `DeleteBehavior.Restrict` everywhere (same reasoning as
  `recipe_ingredients`: don't cascade-delete/silently orphan a user's log, favorites,
  or plan history). `PlannedMealRepository` additionally has a `ListForRangeAsync`
  (`PlanDate` between two dates) alongside the usual `ListForDayAsync`, needed by the
  Shopping list query — both `Include(p => p.Item.Recipe!).ThenInclude(r =>
  r.Ingredients).ThenInclude(ri => ri.Ingredient)` so a recipe-based planned meal's
  full ingredient breakdown is loaded, not just the recipe itself.
  No new table for the shopping list itself — it's computed on demand from
  `PlannedMeal`/`Recipe`/`RecipeIngredient` at query time, per `database-design.md`.

- **`Food.Api`** — wired: `AddApplication()` + `AddInfrastructure()` in `Program.cs`.
  Endpoints:
  - `POST /api/v1/ingredients`, `GET /api/v1/ingredients?search=&tag=`,
    `GET /api/v1/ingredients/{id}`
  - `POST /api/v1/users`, `PUT /api/v1/users/{userId}/profile`,
    `GET /api/v1/users/{userId}/nutrition-target`,
    `PUT /api/v1/users/{userId}/nutrition-target` (manual override)
  - `POST /api/v1/recipes`, `GET /api/v1/recipes?search=`, `GET /api/v1/recipes/{id}`
  - `POST /api/v1/users/{userId}/meal-logs` (body: `logDate`, `mealSlot`, plus either
    `recipeId`+`servingsCount` or `ingredientId`+`quantityGrams`),
    `GET /api/v1/users/{userId}/meal-logs?date=` (defaults to today)
  - `GET /api/v1/users/{userId}/dashboard?date=` (defaults to today; 404 if no
    `NutritionTarget` exists yet for that user/date)
  - `POST /api/v1/users/{userId}/favorite-meals` (body: `displayName`, plus either
    `recipeId`+`servingsCount` or `ingredientId`+`quantityGrams`),
    `GET /api/v1/users/{userId}/favorite-meals`,
    `DELETE /api/v1/users/{userId}/favorite-meals/{id}` (404 if missing *or* owned by
    a different `userId`, 204 on success)
  - `POST /api/v1/users/{userId}/activity-logs` (body: `logDate`, `activityType`,
    `durationMinutes`, `caloriesBurned`),
    `GET /api/v1/users/{userId}/activity-logs?date=` (defaults to today)
  - `POST /api/v1/users/{userId}/planned-meals` (body: `planDate`, `mealSlot`, plus
    either `recipeId`+`servingsCount` or `ingredientId`+`quantityGrams`),
    `GET /api/v1/users/{userId}/planned-meals?date=` (defaults to today)
  - `GET /api/v1/users/{userId}/planned-dashboard?date=` (defaults to today; 404 if no
    `NutritionTarget` exists yet for that user/date — same contract as `/dashboard`)
  - `GET /api/v1/users/{userId}/shopping-list?startDate=&endDate=` (both required,
    query-string bound `DateOnly`; `endDate < startDate` is a validation error)
  JSON enums serialize as strings (`JsonStringEnumConverter`), matching DB storage.
  **Scalar** interactive API docs wired at `/scalar` in Development (`.claude/launch.json`
  lets the `food-api` config be previewed via the Browser tooling). No auth yet.
  **Known gap**: no global exception-handling middleware — an application-level error
  (e.g. `CreateRecipe` referencing a nonexistent ingredient, or `LogMealCommand`
  failing FluentValidation's exactly-one-source rule) surfaces as a raw `500` with a
  full stack trace in the response body, not a clean `400`/`404` problem-details
  response. Reconfirmed this session via `LogMeal` with both/neither of
  `recipeId`/`ingredientId`. Not yet fixed; worth doing before this is exposed beyond
  local dev.
  Verified end-to-end against live local Postgres for all nine slices — including
  logging one ingredient-based and one recipe-based meal, fetching the day's logs,
  saving/listing/deleting favorites, confirming a second user gets 404 (not someone
  else's data) when deleting a favorite they don't own, logging a 300-kcal activity
  and confirming the dashboard's target went from 2509 → 2809 kcal (`remaining`
  shifting by the same +300) while protein/carbs/fat/fiber targets stayed untouched,
  and planning a 1-serving portion of the 2-serving "Banana Oat Bowl" recipe plus a
  raw 100g banana across two future days — confirming both the planned-dashboard math
  and (after catching and fixing the recipe-scaling bug above) the shopping list
  correctly totaling 200g banana (100g recipe-scaled + 100g direct) and 105g oats
  (25g recipe-scaled + 80g direct) across the range.

All 45 tests pass (no new automated tests added for Logging/Dashboard/Favorites/
Activities/Planning/ShoppingList, consistent with how Recipes and Users shipped — see
the `Food.Application.Tests` note above); solution builds clean (0 warnings/errors) in
Debug and Release.

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

**The full business-logic roadmap from `business-description.md` is now built:**

- [x] Ingredients: create, list/search, get by id
- [x] Users & profile: create user, set profile + auto-calculate target, get target,
      manual target override
- [x] Recipes: create (ingredients + quantities), get (computed macros), list
- [x] Meal logging: log a meal (raw ingredient or recipe portion), get a day's logs,
      **daily dashboard query** (macros vs. target, now including the activity-calorie
      adjustment — see Activity logging below)
- [x] Favorites: save/list/remove a favorite meal (does **not** yet include one-tap
      *logging* a meal directly from a favorite — `LogMeal` still only accepts
      `recipeId`/`ingredientId`, not a `favoriteMealId`; `business-description.md`
      calls this out as a third valid source but it wasn't part of this slice's scope)
- [x] Activity logging: log activity, adjust the day's calorie target
- [x] Weekly planner: plan a meal for a future date, check planned macros vs. target
      (no remove/edit of a planned meal yet — see the Planning note above)
- [x] Shopping list: aggregate ingredients across planned meals in a date range

There is no next roadmap item to pull from `business-description.md` — every screen it
describes has a working API surface. What's left is cross-cutting hardening, not new
business logic:

- **Auth** — deliberately deferred throughout (decided 2026-08-07), but now the most
  load-bearing gap: all nine slices trust a client-supplied `userId`/`CreatedByUserId`
  with zero verification, and `docs/database-design.md` already flags that `bigint`
  identity PKs are enumerable. With the roadmap complete, this is the natural next
  step — there's nothing left to build that doesn't need it.
- **Global exception handling** — see the `Food.Api` gap noted above. Reconfirmed
  across four different slices this session/previous sessions (Recipe, Logging,
  ShoppingList validation failures) as the same raw-500 behavior; still not fixed.
  Cheap, mechanical, and worth doing before or alongside auth.
- **VPS hardening** — finish SSH (`PermitRootLogin prohibit-password`,
  `PasswordAuthentication no`) and firewall (ufw), left open while getting Postgres
  running. Worth doing before this server handles anything real.
- Test coverage — `Food.Application.Tests` still only covers the original
  `CreateIngredient` slice (13 tests); every slice added since (8 of them) was
  verified manually via curl instead. Fine for a solo project moving fast, but worth
  a dedicated pass at some point rather than deferring indefinitely.

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
