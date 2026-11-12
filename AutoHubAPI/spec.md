# AutoHubAPI — специфікація проєкту

## 1. Призначення

AutoHubAPI — REST API бекенд для майданчика продажу комерційного транспорту (вантажівки, фургони, рефрижератори, самоскиди тощо) з обробкою заявок (лідів) від клієнтів. Окремо в тій самій базі розміщено навчальний модуль **TestHub** — тести з питаннями та варіантами відповідей.

Поточний стан: шар даних, міграції, сідинг і базова інфраструктура готові. Для TestHub реалізовано `TestService` (CRUD тестів) і `TestsController` (список, інформація, проходження та перевірка тесту). Контролерів маркетплейсу ще немає (є шаблонний `WeatherForecastController`).

## 2. Технології

| Категорія | Технологія | Версія |
|---|---|---|
| Платформа | .NET / C# | `net9.0`, `Nullable` та `ImplicitUsings` увімкнено |
| Веб-фреймворк | ASP.NET Core Web API (контролери) | 9 |
| ORM | Entity Framework Core | 9.0.21 / 9.0.22 |
| СУБД | PostgreSQL (хмарний Neon) через Npgsql | Npgsql.EFCore.PostgreSQL 9.0.4 |
| Фонові задачі | Quartz.NET | 3.16.1 |
| Документація API | Swashbuckle (Swagger / Swagger UI) | 9.0.6 |
| Рішення | `AutoHubAPI.slnx` (новий XML-формат solution) | — |

## 3. Структура рішення

```
AutoHubAPI.slnx
├── Domain/            — сутності, AppDbContext, міграції EF Core
│   ├── Entities/          (маркетплейс)
│   ├── Entities/TestHub/  (модуль тестування)
│   └── Migrations/
├── Application/       — інтерфейси, generic-репозиторій, специфікації, сервіси, DTO
│   ├── Interfaces/
│   ├── Models/            (DTO / Request, напр. Models/Tests)
│   ├── Repositories/
│   └── Services/
├── Infrastructure/    — реєстрація DI, конфігурація Quartz, фонові задачі
│   ├── ProgramConfiguration/DependencyInjection.cs
│   └── Jobs/DbSeedJob.cs
└── AutoHubAPI/        — хост-застосунок (Program.cs, контролери, appsettings)
```

## 4. Залежності між проєктами

```
AutoHubAPI ──► Infrastructure ──► Application ──► Domain
```

| Проєкт | Посилається на | NuGet-пакети |
|---|---|---|
| Domain | — | EFCore.Design, EFCore.Tools, Npgsql.EntityFrameworkCore.PostgreSQL |
| Application | Domain | Microsoft.EntityFrameworkCore 9.0.22 |
| Infrastructure | Application | Quartz.AspNetCore, Quartz.Extensions.DependencyInjection, Quartz.Extensions.Hosting, Swashbuckle.AspNetCore |
| AutoHubAPI | Infrastructure | EFCore.Design, Swashbuckle.AspNetCore |

## 5. Модель даних

### 5.1 Маркетплейс

Усі сутності успадковують `BaseEntity<int>` → `IEntity<T>` (`Id`, `IsDeleted`, `DateCreated`). Таблиці мають імена у snake_case (через `[Table]`).

| Сутність | Таблиця | Ключові поля | Зв'язки / індекси |
|---|---|---|---|
| `VehicleEntity` | `vehicles` | Model, Subtitle, Year, Mileage, EngineVolume `numeric(3,1)`, Price `numeric(12,2)` (USD), Vin (17), Description, CreatedAt, UpdatedAt | FK: Brand, BodyType, FuelType, Status (за замовч. 1 = «В наявності»); 1:N Images, Specifications, Leads; індекси за BrandId, BodyTypeId, FuelTypeId, Price, Year, (StatusId, CreatedAt) |
| `VehicleImageEntity` | `vehicle_images` | Url, IsMain, SortOrder | FK Vehicle; індекс (VehicleId, SortOrder) |
| `VehicleSpecificationEntity` | `vehicle_specifications` | Name, Value, SortOrder | FK Vehicle — довільні характеристики «ключ–значення» |
| `BrandEntity` | `brands` | Name (unique) | — |
| `BodyTypeEntity` | `body_types` | Name, Slug (unique), ImageUrl, SortOrder | — |
| `FuelTypeEntity` | `fuel_types` | Name (unique) | — |
| `VehicleStatusEntity` | `vehicle_statuses` | Name (unique) | В наявності / Заброньовано / Продано |
| `LeadEntity` | `leads` | Name, Phone, Message, CreatedAt | FK Vehicle (nullable — загальна заявка), FK Status (за замовч. 1); індекс (StatusId, CreatedAt) |
| `LeadStatusEntity` | `lead_statuses` | Name (unique) | Нова / В роботі / Виконана / Відхилена |

### 5.2 TestHub

Сутності **не** успадковують `BaseEntity` (немає soft delete, імена таблиць у PascalCase).

```
TestEntity (Tests) 1──N QuestionEntity (Questions) 1──N AnswerOptionEntity (AnswerOptions)
```

- `TestEntity`: Title, Description, CreatedAt, TimeLimitMinutes (1–600, необов'язкове).
- `QuestionEntity`: Text, OrderIndex, Points (1–100), TestId.
- `AnswerOptionEntity`: Text, IsCorrect, QuestionId (допускається кілька правильних відповідей).

Доступ до даних TestHub — напряму через `AppDbContext` у `TestService` (не через `IGenericRepository`). Видалення тесту — **фізичне**, питання й відповіді видаляються каскадно.

### 5.3 Міграції

Розташовані в `Domain/Migrations`:
1. `20261029085014_Init Database` — схема маркетплейсу.
2. `20261105090549_AddTestHub` — таблиці TestHub.

## 6. Основні архітектурні рішення

1. **Багатошарова архітектура (у стилі Clean/Onion)** з чотирма проєктами та односпрямованими залежностями. Відхилення від класичної схеми: `AppDbContext` і міграції лежать у `Domain`, а `Application` напряму використовує EF Core та `AppDbContext` (окремого шару Persistence немає).

2. **Конфігурація EF Core через Data Annotations** (`[Table]`, `[Index]`, `[ForeignKey]`, `[Column(TypeName)]`, `[Range]`, `[MaxLength]`) замість Fluent API; `OnModelCreating` не перевизначено.

3. **Generic Repository + Specification**:
   - `IGenericRepository<TEntity, TKey>` / `GenericRepository` — CRUD, `ListAsync(ISpecification)`, `AsQurable()`; зареєстрований як open generic (`AddScoped(typeof(IGenericRepository<,>), ...)`).
   - `BaseSpecification<T>` інкапсулює умову фільтрації, `Include`-и та сортування.
   - Працює тільки з сутностями, що реалізують `IEntity<TKey>` (тобто не з TestHub).

4. **Soft delete**: `DeleteAsync` встановлює `IsDeleted = true`; методи читання за замовчуванням відфільтровують видалені записи (параметр `isSoft` дозволяє отримати видалені). Глобальних query filters у DbContext немає — фільтрація відбувається лише в репозиторії.

5. **Централізована реєстрація DI** — метод розширення `AddInfrastructureServices` в `Infrastructure` реєструє все: CORS, репозиторії, сервіси, Quartz, DbContext, контролери, Swagger. `Program.cs` залишається мінімальним.

6. **Автоматичні міграції та сідинг під час старту через Quartz**: `DbSeedJob` запускається одразу (`StartNow`) і викликає `ISeederService`:
   - `UpdateDatabase()` → `Database.MigrateAsync()`;
   - `SeedTestHubAsync` — читає `AutoHubAPI/SeedData/tests.json` (тест «Основи C#» — 22 питання, «Entity Framework Core» — 25 питань) і додає відсутні тести (за `Title`) та питання з відповідями (за `Text`); оновлює Description/TimeLimitMinutes існуючих тестів. Файл копіюється в output при build і publish (`CopyToOutputDirectory`/`CopyToPublishDirectory` у `AutoHubAPI.csproj`).
   Методи сідингу маркетплейсу ідемпотентні (нічого не роблять, якщо таблиця вже має записи). Методи сідингу маркетплейсу (`SeedBrandsAsync`, `SeedVehiclesAsync` тощо, 17 авто + 9 заявок) реалізовані, але зараз у задачі не викликаються. Старий варіант сідингу в `Program.cs` закоментовано.

7. **CORS-політика `AllowAll`** — будь-яке джерело з `AllowCredentials`; призначено для розробки.

8. **Swagger UI увімкнено в усіх середовищах** (без перевірки `IsDevelopment`).

9. **Primary constructors (C# 12)** використовуються для впровадження залежностей у сервісах, репозиторіях і задачах.

## 7. API

| Метод | Маршрут | Опис | Відповідь |
|---|---|---|---|
| GET | `/api/tests` | Список усіх тестів | `TestItemDto[]` |
| GET | `/api/tests/{id}` | Інформація про тест (к-сть питань, макс. бал) | `TestInfoDto` / 404 |
| GET | `/api/tests/{id}/pass` | Питання й варіанти для проходження (без `IsCorrect`, з `IsMultipleChoice`) | `TestPassDto` / 404 |
| POST | `/api/tests/{id}/submit` | Перевірка відповідей (`TestSubmitRequest`); питання зараховується лише при точному збігу набору правильних варіантів, часткових балів немає | `TestResultDto` / 404 |

Моделі (`Application/Models/Tests`):
- `TestItemDto` (record): Id, Title, Description, TimeLimitMinutes, CreatedAt, QuestionsCount.
- `TestSaveRequest`: Title (обов'язкове, ≤200), Description (≤1000), TimeLimitMinutes (1–600) — валідація через Data Annotations.

`ITestService` також містить `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync` — ендпоінтів для них ще немає.

## 8. Конфігурація та запуск

- Рядок підключення `ConnectionStrings:DefaultConnection` в `AutoHubAPI/appsettings.json` (віддалена БД Neon PostgreSQL).
- Профіль запуску: `http://localhost:5230`, стартова сторінка `/swagger`.

```bash
dotnet build AutoHubAPI.slnx
dotnet run --project AutoHubAPI
dotnet ef migrations add <Name> --project Domain --startup-project AutoHubAPI
```

## 9. Відомі проблеми та технічний борг

- Облікові дані БД збережені в `appsettings.json` під git — варто перенести в user-secrets / змінні середовища та змінити пароль.
- Різні версії EF Core: 9.0.22 (Application) та 9.0.21 (Domain, AutoHubAPI).
- `DbSeedJob` не сідить дані маркетплейсу.
- Немає контролерів маркетплейсу; Create/Update/Delete тестів є в `TestService`, але не в API; результати проходження не зберігаються, ліміт часу на сервері не перевіряється.
- Немає FluentValidation (коментар у DI без реалізації — використовуються лише Data Annotations), автентифікації (`UseAuthorization` без схеми), глобальної обробки помилок.
- Немає тестового проєкту.
- `BaseEntity.DateCreated` використовує `DateTime.Now` з позначкою UTC (локальний час, позначений як UTC); у `VehicleEntity`/`LeadEntity` додатково дублюється `CreatedAt`.
- `GenericRepository.UpdateAsync`/`DeleteAsync` одразу викликають `SaveChangesAsync`, а `AddAsync` — ні (непослідовна семантика Unit of Work).
- Опечатка в назві методу `AsQurable` та зайвий `using Microsoft.VisualBasic.FileIO` у `VehicleEntity`.
