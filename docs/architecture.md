# Architecture

Status: **draft** — decisions marked *Proposed* are not final and should be confirmed
before we scaffold the corresponding code.

## Style: Clean Architecture (layered, dependency-inward)

Five projects, each only allowed to depend on the layers listed below it:

```
Food.Api             <- composition root: HTTP endpoints, auth middleware, DI wiring
Food.Infrastructure   <- EF Core, external services (barcode lookup, SSO providers)
Food.Application      <- use cases (CQRS-style handlers), DTOs, validation
Food.Domain           <- entities, value objects, domain logic. No external dependencies.
Food.Shared           <- cross-cutting types with no business logic. No dependencies.
```

Dependency rule: `Shared` depends on nothing, and any other layer (including `Domain`)
may depend on it. `Domain` depends only on `Shared`. `Application` depends on `Domain` +
`Shared`. `Infrastructure` depends on `Application` + `Domain` + `Shared`. `Api` depends
on all of the above and wires concrete implementations to interfaces at startup.

Rationale: the domain (macro calculations, recipe composition, planning rules) should
be testable and framework-agnostic, independent of EF Core or ASP.NET Core. This also
keeps `Api` controllers/endpoints thin — they translate HTTP <-> use case, nothing more.

## Layers in detail

### Food.Shared
- Cross-cutting types with no business logic and no dependency on any other Food
  project: common constants, extension methods, a `Result<T>`/error-wrapper type,
  pagination request/response models, shared enums.
- Exists because some types (e.g. pagination models, result wrappers) are needed
  identically across layers — putting them in `Domain` would force `Application`/
  `Infrastructure` to depend on domain concepts just to reuse a plumbing type.
- Rule of thumb: if a type encodes a business rule, it belongs in `Domain`, not here.

### Food.Domain
- Entities: `User`, `Ingredient`, `Recipe`, `MealLog`, `DailyTarget`, `PlannedMeal`, etc.
- Value objects: `MacroBreakdown` (calories/protein/carbs/fat/fiber), `Portion`.
- Domain services: macro target calculation (BMR/TDEE-based), recipe macro aggregation.
- No dependency on EF Core, ASP.NET Core, or any package beyond the BCL.

### Food.Application
- Use case handlers (CQRS-style: one command/query per use case) via **MediatR**
  *(Decided)*.
- DTOs for input/output at the use case boundary (kept separate from Domain entities).
- Validation via **FluentValidation** *(Proposed)*.
- Defines interfaces the Infrastructure layer implements (`IIngredientRepository`,
  `IBarcodeLookupService`, `IClock`, etc.) — Application never references Infrastructure
  directly.

### Food.Infrastructure
- EF Core `DbContext`, migrations, repository implementations.
- Database: **PostgreSQL** *(Decided)*.
- External integrations: barcode/ingredient lookup API, Google/Apple SSO token
  validation.

### Food.Api
- ASP.NET Core Web API, versioned routes (`/api/v1/...`).
- Auth: JWT bearer tokens issued after email/password or SSO login *(Proposed —
  ASP.NET Core Identity for local user management + external login providers for
  Google/Apple, issuing our own JWT for subsequent API calls)*.
- Controllers/minimal API endpoints stay thin: map HTTP request to an Application
  command/query, return the result. No business logic here.

## Testing strategy (Proposed)

- `Food.Domain.Tests` — pure unit tests, no mocking needed given no external deps.
- `Food.Application.Tests` — unit tests for handlers, with Infrastructure interfaces
  mocked/faked.
- Test framework: **NUnit** *(Decided)*.
- Integration tests (`WebApplicationFactory` + a real/test database) are deliberately
  **out of scope for now** — revisit once the API has enough surface to make them
  worthwhile.

## Decided

- [x] Database engine — **PostgreSQL**.
- [x] Mediator/CQRS library — **MediatR**.
- [x] Test framework — **NUnit**.

## Open decisions

These need explicit confirmation before the corresponding code is scaffolded:

- [ ] Validation library — FluentValidation assumed, not confirmed.
- [ ] Auth approach — ASP.NET Core Identity + JWT assumed, not confirmed.
- [ ] Hosting/deployment target — not discussed yet.

## Architecture Decision Records

Significant decisions (once confirmed) should get a short ADR under `docs/adr/` so the
reasoning is preserved. None exist yet.
