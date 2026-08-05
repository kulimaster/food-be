# CLAUDE.md

Guidance for Claude Code (and any contributor) working in this repository.

## Project overview

**Food** is a REST API backend for a meal-planning / nutrition-tracking app. It supports
daily macro tracking (calories, protein, carbs, fat, fiber), an ingredients database,
custom recipes, favorite meals, and a weekly meal planner with shopping-list generation.
See the original design brief in project conversation history for full screen-by-screen
scope (dashboard, ingredients DB, recipe builder, weekly planner).

## Stack & status

- **Language / runtime:** C# on .NET 10 (SDK pinned in `global.json`).
- **API style:** REST.
- Database, auth provider integration, and architecture layering are documented in
  [`docs/architecture.md`](docs/architecture.md) — check there before assuming a choice
  hasn't been made.
- Business rules and domain scope are documented in
  [`docs/business-description.md`](docs/business-description.md).
- Schema design is documented in [`docs/database-design.md`](docs/database-design.md).
- Solution: `Food.slnx`, five projects under `src/` (`Food.Shared`, `Food.Domain`,
  `Food.Application`, `Food.Infrastructure`, `Food.Api`). `Food.Domain` is fully
  implemented; `Food.Application` has one vertical slice (create ingredient);
  `Food.Infrastructure` and `Food.Api` are still empty scaffolds. See
  [`docs/progress.md`](docs/progress.md) for the current status and what's next —
  **read that file first** when picking this project back up.

## Conventions already in place

- **Code style / naming:** enforced via [`.editorconfig`](.editorconfig). Naming rules
  and correctness-oriented rules are `warning` severity; purely aesthetic preferences
  (e.g. `var` usage) are `suggestion`.
- **Build settings:** [`Directory.Build.props`](Directory.Build.props) enables nullable
  reference types, implicit usings, and .NET analyzers solution-wide. Warnings are
  treated as errors in `Release` builds only (Debug stays fast to iterate on).
- **SDK version:** pinned in [`global.json`](global.json), `rollForward: latestPatch`.

## Build / test / run

```bash
dotnet build                 # build the whole solution (Debug)
dotnet build -c Release      # Release build; warnings are treated as errors here
dotnet test                  # run all test projects
dotnet test tests/Food.Domain.Tests   # run a single test project
```

No runnable API yet (`Food.Api` has no endpoints beyond the template). Update this
section once there's something to `dotnet run`.

## Working agreements for this repo

- **Small, incremental steps.** One change at a time, not batched or speculative work.
- **Ask when something is ambiguous or a judgment call** (naming, severity levels,
  library choices) rather than assuming — the user prefers to be asked.
- **Always confirm before `git commit`**, and confirm again separately before `git push`.
- Don't add abstractions, config, or tooling beyond what the current step calls for.

## Related docs

- [`docs/progress.md`](docs/progress.md) — **read this first**: current implementation
  status, what's built/tested, and the next planned step.
- [`docs/architecture.md`](docs/architecture.md) — architecture style, layering, and
  technology decisions (including open/proposed ones still needing confirmation).
- [`docs/business-description.md`](docs/business-description.md) — business rules and
  domain scope.
- [`docs/database-design.md`](docs/database-design.md) — ER diagram and schema.
