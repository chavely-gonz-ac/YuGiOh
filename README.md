# 🃏 Yu-Gi-Oh! Tournament Management Backend

> A backend system for managing Yu-Gi-Oh! tournaments, players, and authentication — built with **.NET 7**, **MediatR**, and **FluentValidation** using a clean architecture.

---

## ✅ Implemented Features

### 🧩 Architecture & Core

-**Clean Architecture** with separated layers:

-`Domain` → entities, DTOs, exceptions, and interfaces

-`Application` → MediatR commands, queries, and validators

-`Infrastructure` → Redis caching, email, and CSC API integrations

-`WebAPI` → REST endpoints with Swagger UI

-**CQRS Pattern** implemented via MediatR pipeline.

-**Centralized Validation** using `ValidationBehavior<TRequest, TResponse>` and `FluentValidation`.

-**Custom Exception Handling** with unified `APIException` model.

---

### 🔐 Authentication & Authorization

-**User Registration** with validation and data encapsulation (`RegisterCommand`).

-**Email Confirmation** through `SendConfirmationEmailCommand` and domain-driven email provider/sender.

-**User Login (Authentication)** using `AuthenticateCommand` and domain `IAuthenticationHandler`.

-**Token Refresh Flow** via `RefreshTokenCommand`.

-**Email Confirmation Validation** with `ConfirmEmailQuery`.

All authentication actions are validated with dedicated validators.

---

### ✉️ Email System

- Configurable **IEmailSender** and **IEmailProvider** interfaces.
- Automatic **confirmation email generation** with customizable templates.
- Full integration in the registration pipeline.

---

### 🗃️ Data Validation

-**Comprehensive FluentValidation Rules** for:

- User registration (email, password strength, names, surnames, roles, IBAN for sponsors)
- Address details (country, state, city, building, apartment)
- Authentication inputs (handler, password, IP address)
- Token refresh and email confirmation requests

All rules are enforced before command execution via the MediatR validation behavior.

---

### 🌐 External Services Integration

-**Country-State-City (CSC) API Integration** for real-world geographical data.

-**Redis Caching** for API responses and frequently requested data.

---

### 🧠 Domain Features

- Domain models for:

-**Users**

-**Addresses**

-**Roles**

-**Decks** (structure, archetype, card counts)

-**Tournaments** (concept defined in Domain layer for future expansion)

All domain-level operations are abstracted via interfaces in `YuGiOh.Domain.Services`.

---

### ⚙️ Developer Experience

-**Dependency Injection** configured in `ServiceExtension.AddApplicationLayer()`.

-**Automatic Validator Discovery** via reflection.

-**Swagger UI** documentation enabled in WebAPI project.

-**Consistent project organization** following DDD conventions.

---

## 🧾 Summary

| Layer                    | Key Implementations                               |

| ------------------------ | ------------------------------------------------- |

| **Domain**         | DTOs, Entities, APIException, Service Interfaces  |

| **Application**    | Commands, Queries, Validators, Pipeline Behaviors |

| **Infrastructure** | Redis, Email, CSC Integration                     |

| **WebAPI**         | Controllers, Swagger, Dependency Configuration    |

---

## 👤 Author

**[Chavely]**

Backend Junior Developer | .NET & Clean Architecture

📧 [chavely.gonz.acl@example.com]

---
