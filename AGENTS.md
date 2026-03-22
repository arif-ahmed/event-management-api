# AGENTS    .md — EMS API Architecture Guide

## Project Overview
Event Management System — Modular Monolith, Clean Architecture, CQRS, DDD.
Assignment-grade but production-quality .NET Core REST API.

---

## Solution Structure

```
src/
├── EMS.SharedKernel/       # Primitives only — AggregateRoot, Result<T>, IDomainEvent
├── EMS.Domain/             # Aggregates, Entities, Value Objects, Domain Events, Exceptions
├── EMS.Application/        # Commands, Queries, Handlers, Interfaces, Behaviors
├── EMS.Infrastructure/     # Dapper repos, RabbitMQ, EF migrations, SignalR, Cache, Auth
└── EMS.API/                # Controllers (v1), Hubs, Middleware, Program.cs

tests/
├── EMS.UnitTests/          # Domain rules + Application handlers (TDD)
└── EMS.IntegrationTests/   # DB + RabbitMQ + E2E scenarios
```

---

## Dependency Rules (Non-Negotiable)

```
API → Application → Domain → SharedKernel
Infrastructure → Application → Domain → SharedKernel
```

- `EMS.Domain` has zero infrastructure dependencies
- `EMS.Application` defines all interfaces — Infrastructure implements them
- No query handler reads from command-side tables directly

---

## SharedKernel — Allowed Contents Only

```
EMS.SharedKernel/
├── AggregateRoot.cs        # Holds uncommitted domain events list
├── Entity.cs               # Base with Id + equality
├── ValueObject.cs          # Equality by value
├── IDomainEvent.cs         # Marker interface
├── DomainEventBase.cs      # EventId, OccurredAt, CorrelationId
├── Result.cs               # Result<T> with Success/Failure + Error type
├── Error.cs                # ErrorCode + Message + ErrorType enum
└── PagedResult.cs          # Pagination wrapper
```

**Do not add anything else here.** If unsure, put it in Domain.

---

## Key Technology Decisions

| Concern | Decision |
|---|---|
| ORM | EF Core — migrations + Identity only. Dapper — all queries |
| CQRS | MediatR — Commands, Queries, Notifications |
| Validation | FluentValidation via MediatR Pipeline Behavior |
| Messaging | RabbitMQ fanout exchange — domain events after DB commit |
| Cache | Microsoft.Extensions.Caching.Hybrid (memory + distributed) |
| Auth | JWT (short-lived) + Refresh Token (strict rotation + reuse detection) |
| Real-time | SignalR — report job progress + waitlist/booking notifications |
| File Storage | IFileStorageService abstraction — LocalFileStorageService (default) |
| Logging | Serilog — structured, CorrelationId on every log entry |
| Testing | TUnit — unit tests. Testcontainers — integration tests |
| DB | SQL Server (LocalDB dev / full prod) |
| Rate Limiting | Booking endpoints only — IP-based + UserId-based |

---

## Domain Event Flow

```
Command Handler
  → Aggregate raises DomainEvent (appended to uncommitted list)
  → UnitOfWork.CommitAsync()
      → SQL transaction (state + DomainEventLog insert)
      → On success: IMessagePublisher publishes to RabbitMQ fanout
  → RabbitMQ Consumer receives event
      → Updates Read Model tables
      → Sends SignalR notification (if applicable)
```

**No Outbox Pattern. No Event Sourcing. State DB is source of truth.**

---

## Concurrency Strategy

- `RowVersion` column on `Events` and `Bookings` tables
- Booking save: `UPDATE Events SET BookedSeats = @new WHERE Id = @id AND RowVersion = @expected`
- 0 rows affected → throw `ConcurrencyException` → return `409 Conflict`
- Client is responsible for retry

---

## Booking Business Rules (inside Aggregate only)

1. Event must exist and not be deleted
2. No duplicate booking per user per event — enforced by DB UNIQUE + aggregate check
3. `BookedSeats >= MaxSeats` → Status = `Waitlisted`, assign next `WaitlistPosition`
4. On cancellation → `WaitlistRepository.GetNextInLine()` → auto-promote → `WaitlistPromotedDomainEvent`
5. Promoted user receives SignalR notification

---

## JWT Refresh Token — Strict Rotation

1. On `/auth/refresh` — find token by hash in DB
2. Token already used (`IsUsed = true`) → **revoke ALL tokens for that user** → `401`
3. Token expired → `401`
4. Valid → mark old token `IsUsed = true`, issue new access + refresh token pair

---

## MediatR Pipeline Order

```
LoggingBehavior → ValidationBehavior → AuditBehavior → Handler
```

- `ValidationBehavior` — FluentValidation, throws on failure → `400`
- `AuditBehavior` — writes to `AuditLogs` table for write commands only
- `LoggingBehavior` — logs request + response with CorrelationId

---

## Database Tables (Summary)

### Command Side
- `Users` — Id, Name, Email, PasswordHash, Role, RefreshToken, RowVersion
- `Events` — Id, Title, Description, Location, StartTime, EndTime, MaxSeats, BookedSeats, CreatedByUserId, IsDeleted, RowVersion
- `Bookings` — Id, UserId, EventId, Status, WaitlistPosition, RowVersion, UNIQUE(UserId, EventId)
- `AuditLogs` — Id, UserId, Action, ResourceType, ResourceId, Details, CorrelationId, Timestamp
- `DomainEventLog` — Id, EventId(unique), AggregateId, EventType, Payload, CorrelationId, OccurredAt
- `ReportJobs` — Id, RequestedByUserId, ReportType, Status, Progress, FilePath, ErrorMessage, CreatedAt, CompletedAt
- `IdempotencyKeys` — Key, CreatedAt, ExpiresAt

### Read Model (same DB, separate tables)
- `ReadModel_Events` — denormalized event + booking count + organizer name
- `ReadModel_Bookings` — denormalized booking + event title + user name
- `ReadModel_Attendance` — for admin attendance report queries

---

## API Versioning

All routes prefixed `/api/v1/`. URL-based versioning only.

### Endpoint Summary
```
POST   /api/v1/auth/register|login|refresh|revoke

GET    /api/v1/events                    # filter: title, location, dateFrom, dateTo, availableOnly, page, sort
GET    /api/v1/events/{id}
POST   /api/v1/events                    # Organizer | Admin
PUT    /api/v1/events/{id}               # Owner | Admin
DELETE /api/v1/events/{id}               # Owner | Admin

POST   /api/v1/events/{id}/bookings      # Member — rate limited
GET    /api/v1/bookings                  # Member own bookings
DELETE /api/v1/bookings/{id}             # Member own cancellation
GET    /api/v1/events/{id}/bookings      # Organizer | Admin

GET    /api/v1/users                     # Admin
PATCH  /api/v1/users/{id}/role           # Admin

GET    /api/v1/reports/events            # Admin
GET    /api/v1/reports/attendance        # Admin
POST   /api/v1/reports/export            # Admin — returns 202 + jobId
GET    /api/v1/reports/status/{jobId}    # Admin — polling fallback
```

---

## Testing Approach

**Always TDD — write test first, then implementation.**

### EMS.UnitTests
```
Domain/
  EventAggregateTests       # overbooking, waitlist assign, ownership
  BookingAggregateTests     # cancel rules, status transitions
Application/
  PlaceBookingHandlerTests  # happy path, not found, duplicate, concurrency
  CancelBookingHandlerTests # success, unauthorized
  RefreshTokenHandlerTests  # valid rotation, reuse detection
Shared/
  Builders/                 # EventBuilder, BookingBuilder, UserBuilder
  Fakes/                    # FakeEventRepository, FakeMessagePublisher
```

### EMS.IntegrationTests
```
Persistence/
  BookingConcurrencyTests   # 10 parallel requests, 1 seat → exactly 1 success
  WaitlistPromoteTests      # cancel → next user promoted atomically
Messaging/
  RabbitMqPublishConsumeTests  # event published → consumer updates read model
Jobs/
  ReportJobLifecycleTests   # Queued → InProgress → Completed → file exists
E2E/
  BookingFlowTests          # register → create event → book → cancel → read model verify
  AuthFlowTests             # login → refresh → reuse detection → revoke
```

---

## Report Generation Flow (Fire & Forget)

```
POST /api/v1/reports/export
  → ReportJob saved (Status: Queued)
  → 202 Accepted { jobId }

BackgroundService (polling)
  → Status: InProgress, SignalR progress: 0%
  → Query data, generate CSV via CsvHelper
  → SignalR progress updates: 25% → 50% → 75%
  → IFileStorageService.SaveAsync()
  → Status: Completed, Progress: 100%, FilePath saved
  → SignalR: { jobId, status: "Completed", downloadUrl }

GET /api/v1/reports/status/{jobId}   ← polling fallback if SignalR unavailable
```

---

## SignalR Events Reference

| Event Name | Trigger | Recipient |
|---|---|---|
| `ReportProgress` | Report job status change | Requesting admin |
| `BookingConfirmed` | Booking placed successfully | Booking user |
| `WaitlistPromoted` | User moved from waitlist to confirmed | Promoted user |

---

## File Storage Abstraction

```
IFileStorageService
  SaveAsync(fileName, stream) → filePath
  GetDownloadUrlAsync(filePath) → url

LocalFileStorageService    ← default implementation (wwwroot/reports/)
# AzureBlobStorageService  ← swap via DI config, same interface
# S3FileStorageService     ← same interface
```

Swap provider by changing DI registration in `Infrastructure/DependencyInjection.cs` only.

---

## Coding Conventions

- Handlers return `Result<T>` — never throw except for truly unexpected errors
- Aggregates throw Domain Exceptions for business rule violations
- No business logic in Controllers or Handlers — only orchestration
- All Dapper queries use parameterized inputs — no string interpolation
- CorrelationId flows from HTTP header → Command → DomainEvent → Log
- One Command/Query per file — folder named after the operation