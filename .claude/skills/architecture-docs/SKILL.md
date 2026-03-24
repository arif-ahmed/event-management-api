---
name: architecture-docs
description: Generate project-specific architecture documentation (spec/architecture.md) with Mermaid diagrams - DDD, CQRS, Clean Architecture, TDD analysis
license: MIT
compatibility: opencode
metadata:
  audience: developers
  workflow: documentation
  architecture: clean-architecture
  patterns: ddd, cqrs, tdd, soa
  languages: dotnet, typescript, python
---

## What I Do

Analyze your project and generate comprehensive architecture documentation at `spec/architecture.md` including:

- **Project Structure Analysis**: Detect layers, modules, and their relationships
- **Pattern Detection**: Identify DDD, CQRS, Clean Architecture, TDD, SOLID patterns
- **Technology Stack**: Identify frameworks, libraries, and tools in use
- **Mermaid Diagrams**: Create project-specific flowcharts, sequence diagrams, ER diagrams
- **Dependency Analysis**: Map out dependencies and enforce clean architecture rules
- **Testing Strategy**: Document testing approaches and coverage recommendations
- **API Documentation**: Map endpoints and their relationships

---

## When to Use Me

Use me when you need to:

- Create initial architecture documentation for a new project
- Document an existing project's architecture
- Understand the structure of an unfamiliar codebase
- Generate visual diagrams for architecture reviews
- Prepare documentation for team onboarding
- Create technical specifications for stakeholders
- Document design decisions and trade-offs

---

## How I Work

```mermaid
flowchart TB
    START[Load Skill] --> ANALYZE[Analyze Project]
    ANALYZE --> DETECT[Detect Patterns]
    DETECT --> DIAGRAMS[Generate Mermaid Diagrams]
    DIAGRAMS --> DOC[Create spec/architecture.md]
    DOC --> REVIEW[Review & Refine]

    subgraph Analysis["Analysis Steps"]
        SCAN[Scan directory structure]
        CS[Parse .csproj / package.json / requirements.txt]
        DEPS[Analyze dependencies]
        PATTERNS[Identify architectural patterns]
        TECH[Identify technology stack]
    end

    subgraph Output["Output Sections"]
        STRUCT[Project Structure]
        ARCH[Architecture Patterns]
        DIAG[Mermaid Diagrams]
        API[API Documentation]
        DB[Database Schema]
        TEST[Testing Strategy]
    end

    SCAN --> CS
    CS --> DEPS
    DEPS --> PATTERNS
    PATTERNS --> TECH
    TECH --> DIAGRAMS

    classDef step fill:#e3f2fd,stroke:#1565c0
    classDef out fill:#e8f5e9,stroke:#2e7d32

    class START,ANALYZE,DETECT,DIAGRAMS,DOC,REVIEW step
    class STRUCT,ARCH,DIAG,API,DB,TEST out
```

---

## Analysis Process

I perform the following analysis on your project:

### 1. **Project Structure Detection**

- Scan for solution files (`.sln`, `.csproj`, `package.json`, `requirements.txt`)
- Identify layers by naming conventions:
  - `API/`, `Web/`, `Server/` → Presentation Layer
  - `Application/`, `Services/`, `Handlers/` → Application Layer
  - `Domain/`, `Core/`, `Entities/` → Domain Layer
  - `Infrastructure/`, `Data/`, `Persistence/` → Infrastructure Layer
  - `Shared/`, `Common/`, `SharedKernel/` → Shared/Kernel Layer

### 2. **Pattern Detection**

- **DDD**: Detect Aggregates (`AggregateRoot`), Value Objects (`ValueObject`), Domain Events (`DomainEvent`, `IDomainEvent`)
- **CQRS**: Detect MediatR usage, Command/Query handlers, separate read/write models
- **Clean Architecture**: Check dependency direction (outer → inner only)
- **TDD**: Check test coverage, test structure
- **SOLID**: Detect interface segregation, dependency injection patterns

### 3. **Technology Stack Identification**

- **.NET**: Framework version from `.csproj`, NuGet packages
- **TypeScript/Node**: Dependencies from `package.json`, framework (Express, NestJS, Next.js)
- **Python**: Dependencies from `requirements.txt`, `pyproject.toml`, framework (Django, FastAPI)
- **Database**: EF Core, Dapper, SQLAlchemy, TypeORM, etc.
- **Messaging**: RabbitMQ, Kafka, Azure Service Bus, etc.
- **Cache**: Redis, MemoryCache, etc.
- **Auth**: JWT, OAuth, Identity, etc.

### 4. **API Endpoint Analysis**

- Scan controllers, route handlers
- Group by resource and HTTP method
- Identify authentication/authorization requirements

### 5. **Database Schema Detection**

- Analyze Entity Framework models
- Detect migrations or SQL files
- Identify relationships and constraints

---

## Output: spec/architecture.md

The generated documentation includes:

### ✅ Auto-Generated Sections (Based on Analysis)

- Project Overview (detected name, description, type)
- Solution Structure (actual directories and their purpose)
- Architecture Patterns (detected patterns with explanations)
- Technology Stack (identified frameworks and versions)
- Dependency Rules (actual dependency graph)
- API Endpoints (detected routes grouped by resource)
- Database Schema (detected entities and relationships)
- Testing Strategy (detected test structure and tools)

### ✏️ Manual Modification Sections (Placeholders)

- Design Decisions (why specific choices were made)
- Architecture Trade-offs (alternatives considered)
- Deployment Strategy (how to deploy)
- Security Considerations (security measures)
- Performance Considerations (caching, optimization)
- Scaling Strategy (how to scale the application)
- Monitoring & Observability (logging, metrics, alerts)
- Future Enhancements (planned improvements)

---

## Mermaid Diagrams Generated

### 1. Project Structure Diagram

```mermaid
flowchart TB
    subgraph src["src/ (detected)"]
        L1[Layer 1 - API/Presentation]
        L2[Layer 2 - Application]
        L3[Layer 3 - Domain]
        L4[Layer 4 - Infrastructure]
        L5[Layer 5 - Shared/Kernel]
    end

    subgraph tests["tests/ (detected)"]
        T1[Unit Tests]
        T2[Integration Tests]
        T3[E2E Tests]
    end

    L1 --> L2
    L4 --> L2
    L2 --> L3
    L3 --> L5

    T1 --> L2
    T1 --> L3
    T2 --> L4

    classDef layer fill:#f5f5f5,stroke:#333
    classDef test fill:#fff9c4,stroke:#f9a825

    class L1,L2,L3,L4,L5 layer
    class T1,T2,T3 test
```

### 2. Dependency Rules Diagram

```mermaid
graph LR
    API[API/Presentation] --> APP[Application]
    INF[Infrastructure] --> APP
    APP --> DOM[Domain]
    DOM --> SHARED[Shared/Kernel]

    classDef layer fill:#e8eaf6,stroke:#3f51b5,stroke-width:2px
    classDef arrow stroke:#3f51b5,stroke-width:2px

    class API,APP,DOM,SHARED,INF layer
```

### 3. CQRS Flow Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant API as API/Controller
    participant M as MediatR/Dispatcher
    participant H as Handler
    participant A as Aggregate
    participant R as Repository
    participant DB as Database
    participant E as Event Bus

    C->>API: Request
    API->>M: Send Command/Query
    M->>H: Execute
    H->>R: Load Aggregate
    R-->>H: Aggregate
    H->>A: Execute Logic
    A->>A: Raise Event
    H->>R: Save
    R->>DB: Persist
    R->>E: Publish Events
    H-->>M: Result
    M-->>API: Response
    API-->>C: Response
```

### 4. Clean Architecture Layers

```mermaid
flowchart TB
    subgraph Domain["Domain Layer (Core Business)"]
        AGG[Aggregates]
        VO[Value Objects]
        DE[Domain Events]
        ENT[Entities]
        EXC[Domain Exceptions]
    end

    subgraph Application["Application Layer (Use Cases)"]
        CMD[Commands]
        QRY[Queries]
        HND[Handlers]
        SERV[Application Services]
        IFACE[Interfaces]
    end

    subgraph Infrastructure["Infrastructure Layer (External)"]
        REPO[Repositories]
        MSG[Messaging]
        CACHE[Caching]
        DBACC[Data Access]
        AUTH[Authentication]
    end

    subgraph API["API/Presentation Layer (Interface)"]
        CTRL[Controllers]
        DTO[DTOs]
        MIDL[Middleware]
        ROTES[Routes]
    end

    API --> Application
    Infrastructure --> Application
    Application --> Domain

    classDef dom fill:#fff3e0,stroke:#ff6f00
    classDef app fill:#e3f2fd,stroke:#0277bd
    classDef inf fill:#e8f5e9,stroke:#2e7d32
    classDef api fill:#f3e5f5,stroke:#7b1fa2

    class AGG,VO,DE,ENT,EXC dom
    class CMD,QRY,HND,SERV,IFACE app
    class REPO,MSG,CACHE,DBACC,AUTH inf
    class CTRL,DTO,MIDL,ROTES api
```

### 5. Testing Strategy Flow

```mermaid
flowchart TB
    subgraph TDD["Test-Driven Development"]
        TEST[Write Test]
        FAIL[Test Fails Red]
        CODE[Write Code]
        PASS[Test Passes Green]
        REFACTOR[Refactor]
    end

    subgraph TestLevels["Test Pyramid"]
        E2E[End-to-End Tests]
        INT[Integration Tests]
        UNIT[Unit Tests]
    end

    TEST --> FAIL
    FAIL --> CODE
    CODE --> PASS
    PASS --> REFACTOR
    REFACTOR --> PASS

    UNIT --> TestLevels
    INT --> TestLevels
    E2E --> TestLevels

    classDef tdd fill:#c8e6c9,stroke:#2e7d32
    classDef level fill:#bbdefb,stroke:#1565c0

    class TEST,FAIL,CODE,PASS,REFACTOR tdd
    class UNIT,INT,E2E level
```

### 6. Domain Event Flow

```mermaid
sequenceDiagram
    participant CMD as Command Handler
    participant AGG as Aggregate
    participant UOW as Unit of Work
    participant DB as Database
    participant PUB as Event Publisher
    participant SUB as Event Subscriber
    participant RM as Read Model

    CMD->>AGG: Execute Command
    AGG->>AGG: Raise Domain Event
    CMD->>UOW: Commit
    UOW->>DB: Save Aggregate State
    UOW->>DB: Save Event Log
    UOW->>PUB: Publish Events
    UOW-->>CMD: Result

    PUB->>SUB: Deliver Event
    SUB->>RM: Update Read Model

    Note over PUB,RM: Events are published<br/>after successful commit
```

### 7. API Endpoint Map

```mermaid
graph TB
    subgraph Public["Public Endpoints"]
        AUTH[POST /auth/login]
        REG[POST /auth/register]
        PUBR[GET /resources]
    end

    subgraph Authenticated["Authenticated Endpoints"]
        PROF[GET /api/v1/profile]
        UPDT[PUT /api/v1/profile]
    end

    subgraph Admin["Admin Endpoints"]
        ADMU[GET /api/v1/admin/users]
        ADMR[GET /api/v1/admin/reports]
    end

    classDef pub fill:#e8f5e9,stroke:#2e7d32
    classDef auth fill:#fff3e0,stroke:#ff6f00
    classDef admin fill:#ffebee,stroke:#c62828

    class AUTH,REG,PUBR pub
    class PROF,UPDT auth
    class ADMU,ADMR admin
```

### 8. Database Schema (Detected)

```mermaid
erDiagram
    %% This will be auto-generated based on detected entities
    ENTITY_1 {
        uuid Id PK
        string Name
        timestamp CreatedAt
        timestamp UpdatedAt
    }

    ENTITY_2 {
        uuid Id PK
        uuid Entity1Id FK
        string Type
        int Status
    }

    ENTITY_1 ||--o{ ENTITY_2 : "has many"
```

---

## Implementation Details

### For .NET Projects

I analyze:
- `.sln` files → project structure
- `.csproj` files → dependencies, framework version
- `Controllers/` → API endpoints
- `Domain/` → aggregates, entities, value objects
- `Application/` → commands, queries, handlers
- `Infrastructure/` → repositories, services
- `MediatR` usage → CQRS pattern
- `FluentValidation` → validation strategy
- Test projects → testing structure

### For TypeScript/Node Projects

I analyze:
- `package.json` → dependencies, framework (Express, NestJS, Next.js)
- `src/` structure → layer organization
- `controllers/` or `routes/` → API endpoints
- `services/` → business logic
- `models/` or `entities/` → data models
- `repositories/` → data access
- Test files → testing approach

### For Python Projects

I analyze:
- `requirements.txt` or `pyproject.toml` → dependencies
- Framework (Django, FastAPI, Flask)
- `models.py` → database models
- `views.py` or `routers/` → API endpoints
- `services/` → business logic
- Test directory (`tests/`) → testing structure

---

## Customization Guide

After generation, you should review and update:

### 📝 Required Manual Updates

1. **Design Decisions Section** (lines marked with `<!-- TODO: -->`)
   - Explain why specific patterns were chosen
   - Document trade-offs considered
   - Reference architectural principles applied

2. **Business Rules Section**
   - Document specific business logic rules
   - Add validation rules
   - Document invariants

3. **Deployment Strategy**
   - How to deploy (Docker, Kubernetes, Cloud)
   - Environment configurations
   - CI/CD pipeline

4. **Security Considerations**
   - Authentication/Authorization approach
   - Data encryption
   - Secrets management
   - Security headers and policies

5. **Performance Considerations**
   - Caching strategy
   - Database indexing
   - Query optimization
   - Async processing

6. **Monitoring & Observability**
   - Logging framework and levels
   - Metrics collection
   - Alerting rules
   - Distributed tracing

7. **Future Enhancements**
   - Planned features
   - Technical debt to address
   - Refactoring opportunities

---

## Usage Example

```
User: "Generate architecture documentation for this project"

Skill (me):
1. Scans project structure
2. Detects patterns:
   - Clean Architecture with 5 layers
   - CQRS with MediatR
   - DDD with Aggregates and Domain Events
   - TDD with unit and integration tests
3. Identifies tech stack:
   - .NET 8
   - EF Core 8 with SQL Server
   - RabbitMQ for messaging
   - SignalR for real-time
   - xUnit for testing
4. Generates spec/architecture.md with:
   - Project overview
   - Mermaid diagrams (structure, dependencies, CQRS flow)
   - Detected API endpoints
   - Database schema
   - Testing strategy
   - Placeholder sections for customization
```

---

## Requirements

- Project must have recognizable structure (layers, tests)
- Supported languages: .NET (primary), TypeScript/Node, Python
- Must be in a git repository for best results

---

## Output Format

The generated `spec/architecture.md` will have:

```markdown
# Architecture Documentation

## Auto-Generated Content
- Project Overview
- Solution Structure
- Architecture Patterns
- Technology Stack
- Dependency Analysis
- API Endpoints
- Database Schema
- Testing Strategy

## Mermaid Diagrams
- Project Structure
- Dependency Rules
- CQRS Flow
- Clean Architecture Layers
- Testing Strategy
- Domain Event Flow
- API Endpoint Map
- Database Schema

## Manual Sections (TODO)
- Design Decisions
- Architecture Trade-offs
- Deployment Strategy
- Security Considerations
- Performance Considerations
- Scaling Strategy
- Monitoring & Observability
- Future Enhancements
```

---

## Notes

- This skill generates **project-specific** documentation based on actual code analysis
- Mermaid diagrams will reflect your actual project structure
- Placeholders are provided for sections that require human judgment
- All diagrams use standard Mermaid syntax compatible with GitHub, GitLab, VS Code
- The documentation should be kept in sync with code changes (consider adding to CI/CD)
