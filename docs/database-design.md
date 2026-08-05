# Database Design

Status: **draft** — first pass derived from [`business-description.md`](business-description.md).
Items marked *(Proposed)* are default choices, not yet confirmed.

Engine: **PostgreSQL** (per [`architecture.md`](architecture.md)). Primary keys are
**`bigint identity`** *(Decided)* — simpler and smaller than GUIDs. Trade-off accepted
knowingly: sequential IDs are enumerable via the API (e.g. `/api/recipes/124` implies
`125` likely exists), so per-user authorization checks at the `Food.Api` layer must be
solid everywhere a resource is fetched by ID — there's no ID-obscurity safety net here.

## Entity-relationship diagram

```mermaid
erDiagram
    USERS ||--o| USER_PROFILES : has
    USERS ||--o{ NUTRITION_TARGETS : has
    USERS ||--o{ INGREDIENTS : creates
    INGREDIENTS }o--o{ INGREDIENT_TAGS : "tagged with"
    USERS ||--o{ RECIPES : creates
    RECIPES ||--o{ RECIPE_INGREDIENTS : contains
    INGREDIENTS ||--o{ RECIPE_INGREDIENTS : "used in"
    USERS ||--o{ FAVORITE_MEALS : saves
    RECIPES |o--o{ FAVORITE_MEALS : "referenced by"
    INGREDIENTS |o--o{ FAVORITE_MEALS : "referenced by"
    USERS ||--o{ MEAL_LOGS : logs
    RECIPES |o--o{ MEAL_LOGS : "referenced by"
    INGREDIENTS |o--o{ MEAL_LOGS : "referenced by"
    USERS ||--o{ ACTIVITY_LOGS : logs
    USERS ||--o{ PLANNED_MEALS : plans
    RECIPES |o--o{ PLANNED_MEALS : "referenced by"
    INGREDIENTS |o--o{ PLANNED_MEALS : "referenced by"

    USERS {
        bigint id PK
        string email UK
        string password_hash
        string display_name
        string timezone
        timestamp created_at
    }
    USER_PROFILES {
        bigint id PK
        bigint user_id FK
        decimal weight_kg
        decimal height_cm
        date date_of_birth
        string sex
        string activity_level
        string goal
        timestamp updated_at
    }
    NUTRITION_TARGETS {
        bigint id PK
        bigint user_id FK
        date effective_from
        int calories_kcal
        decimal protein_g
        decimal carbs_g
        decimal fat_g
        decimal fiber_g
        bool is_manual_override
        timestamp created_at
    }
    INGREDIENTS {
        bigint id PK
        string name
        decimal calories_per_100g
        decimal protein_per_100g
        decimal carbs_per_100g
        decimal fat_per_100g
        decimal fiber_per_100g
        bigint created_by_user_id FK
        timestamp created_at
    }
    INGREDIENT_TAGS {
        bigint id PK
        string name UK
    }
    RECIPES {
        bigint id PK
        string name
        int servings
        bigint created_by_user_id FK
        timestamp created_at
    }
    RECIPE_INGREDIENTS {
        bigint id PK
        bigint recipe_id FK
        bigint ingredient_id FK
        decimal quantity_g
    }
    FAVORITE_MEALS {
        bigint id PK
        bigint user_id FK
        bigint recipe_id FK
        bigint ingredient_id FK
        decimal quantity_g
        string display_name
        timestamp created_at
    }
    MEAL_LOGS {
        bigint id PK
        bigint user_id FK
        date log_date
        string meal_slot
        bigint recipe_id FK
        decimal servings_count
        bigint ingredient_id FK
        decimal quantity_g
        timestamp logged_at
    }
    ACTIVITY_LOGS {
        bigint id PK
        bigint user_id FK
        date log_date
        string activity_type
        int duration_minutes
        int calories_burned
        timestamp logged_at
    }
    PLANNED_MEALS {
        bigint id PK
        bigint user_id FK
        date plan_date
        string meal_slot
        bigint recipe_id FK
        decimal servings_count
        bigint ingredient_id FK
        decimal quantity_g
        timestamp created_at
    }
```

## Table notes

- **users / user_profiles** — split in two because auth identity (email, password) and
  physical profile data (weight, height...) change at different rates and for different
  reasons. `password_hash` is nullable for SSO-only accounts. `timezone` on `users`
  drives the "day boundary" rule from the business doc.
- **nutrition_targets** — a **history table**, not a single mutable row: each row is
  valid `effective_from` a date until superseded. The target for a given day = the most
  recent row where `effective_from <= that day`. This is what lets past dashboard days
  show the target that actually applied then, even after the user's profile changes
  later. `is_manual_override` distinguishes a system-calculated row from a user-typed one.
- **ingredients** — global, shared by all users (per business doc); `created_by_user_id`
  is kept for attribution/auditing, not for scoping visibility.
- **ingredient_tags** / the ingredient↔tag relationship — modeled as many-to-many; the
  diagram shows it directly, the real schema has a join table
  (`ingredient_ingredient_tags`) with no extra columns.
- **recipe_ingredients** — the join table that gives a recipe its composition; this is
  where "total macros" gets computed from (`ingredient.macros_per_100g × quantity_g`,
  summed, divided by `servings`).
- **favorite_meals** and **meal_logs** both point to *either* `recipe_id` *or*
  `ingredient_id`, matching the business rule that a logged meal can be a raw ingredient
  or a recipe portion. A `CHECK` constraint enforces exactly one is set in both tables.
- **activity_logs** — `activity_type` is a free-text string for now, matching the
  decision to skip MET-formula calorie estimation initially. `calories_burned` is
  manually entered and is what adjusts that day's calorie target on the dashboard.
- **planned_meals** — mirrors `meal_logs`: points to *either* `recipe_id` *or*
  `ingredient_id` (enforced via `CHECK`, same as `meal_logs`/`favorite_meals`). This
  matters because the actual goal behind the planner isn't just "assign recipes to
  days" — it's pre-building a day's full nutrition (recipes *and* standalone
  ingredients) to check it hits the day's target *before* the week starts, the same way
  `meal_logs` lets you check it *after* you've eaten. That check (sum of planned macros
  vs. `nutrition_targets` for that date) is a read/query concern, not a new table.

## Deliberately not a table: the shopping list

The shopping list is **computed on demand**, not persisted: aggregate
`recipe_ingredients` (quantity × `planned_meals.servings_count`) for all `planned_meals`
in a selected date range, grouped by ingredient. No `shopping_list_items` table exists.

This is a deliberate simplicity choice: introducing a stored, editable shopping list
means solving a different problem (e.g. a "purchased" checkbox needs to survive the plan
changing underneath it). If check-off/persistence turns out to be needed, that's a
follow-up schema change, not something to build speculatively now.

## Open questions

- [ ] Should `ingredient_tags` be a fixed/seeded list or fully user-extensible?
- [ ] Does `favorite_meals` need its own `display_name`, or always derive the name from
      the linked recipe/ingredient?
- [ ] Retention/editability rules for past `meal_logs`/`activity_logs` (e.g. can a user
      edit a log from 3 months ago?) — not yet discussed.
