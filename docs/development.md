# Розробка

← [README](../README.md)

Як запустити все локально, див. [RUNNING.md](../RUNNING.md). Тут — налаштування середовища з нуля,
робота з міграціями, типові задачі та відомі обмеження.

## Що потрібно встановити

| Інструмент | Версія | Навіщо |
|---|---|---|
| .NET SDK | 10.x | бекенд |
| Node.js | **24 LTS** рекомендовано (25 працює з попередженням) | фронтенд |
| PostgreSQL | 16+ | UserService (зараз служба Windows на порту **5433**) |
| Docker Desktop | | MongoDB у контейнері `contentplatform-mongo` |
| EF Core CLI | `dotnet tool install -g dotnet-ef` | міграції |

## Секрети (User Secrets)

Паролі та ключі **не лежать у репозиторії**. Кожен API-проєкт має `UserSecretsId`, а значення
зберігаються в профілі користувача Windows (`%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json`).
Вони підхоплюються автоматично лише в середовищі **Development**.

Налаштування на новій машині:

```powershell
cd src/UserService/UserService.API
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5433;Database=UserServiceDb;Username=postgres;Password=<пароль>"
dotnet user-secrets set "Jwt:Secret" "<випадковий рядок щонайменше 32 символи>"
dotnet user-secrets set "Jwt:Issuer" "UserService"
dotnet user-secrets set "Jwt:Audience" "MyApp"
```

```powershell
cd src/ContentService/ContentService.API
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDB:DatabaseName" "ContentServiceDb"
dotnet user-secrets set "Jwt:Secret" "<той самий рядок, що в UserService>"
dotnet user-secrets set "Jwt:Issuer" "UserService"
dotnet user-secrets set "Jwt:Audience" "MyApp"
```

Переглянути поточні значення: `dotnet user-secrets list`.

Згенерувати ключ:

```powershell
$b = New-Object byte[] 48; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)
```

## Бази даних

**MongoDB:**

```powershell
docker start contentplatform-mongo
```

Якщо контейнера немає:

```powershell
docker run -d --name contentplatform-mongo -p 27017:27017 mongo:7
```

**PostgreSQL:** базу `UserServiceDb` створюють міграції (див. нижче).

## Міграції EF Core (UserService)

Міграції лежать в `src/UserService/UserService.Infrastructure/Migrations`, а startup-проєкт (з конфігурацією) — `UserService.API`.

Застосувати до бази:

```powershell
dotnet ef database update --project src/UserService/UserService.Infrastructure --startup-project src/UserService/UserService.API
```

Після зміни моделі (`User` або `AppDbContext`):

```powershell
dotnet ef migrations add <НазваЗміни> --project src/UserService/UserService.Infrastructure --startup-project src/UserService/UserService.API
```

Міграції **не застосовуються автоматично** при старті сервісу, команду `database update` треба виконати вручну.

## Збірка

```powershell
dotnet build CleanArchDemo.slnx
```

Збірка має проходити з **0 попереджень**.

Якщо сервіси запущені з Visual Studio, `dotnet build` не зможе перезаписати `.exe` (файл зайнятий).
Зупиніть їх або збирайте в окрему папку: `dotnet build --artifacts-path <папка>`.

**`DirectoryNotFoundException: ...\wwwroot\`** при старті означає, що в `bin` залишився старий маніфест статичних файлів.
Лікується через **Build → Clean Solution → Rebuild**, або видаліть `bin` і `obj` проєкту.

## Як додати нову фічу (приклад: видалення статті)

Порядок — від домену назовні:

1. **Domain:** додайте в `IContentRepository` метод `Task<bool> DeleteAsync(string id, CancellationToken ct)`.
2. **Infrastructure:** реалізуйте його в `ContentRepository` через `_collection.DeleteOneAsync(x => x.Id == id, ct)`.
3. **Application:** створіть `Commands/DeleteContent/`:
   - `DeleteContentCommand(string Id, string UserId) : IRequest`;
   - `DeleteContentHandler`: знайти запис, перевірити `AuthorId == UserId` (інакше `403`), видалити;
   - `DeleteContentCommandValidator`, якщо потрібні правила.
4. **API:** додайте в `ContentController` `[HttpDelete("{id}")]` і передайте `CurrentUserId`.
5. **Помилки:** для «не знайдено» / «не твоє» додайте винятки (`NotFoundException`, `ForbiddenException`)
   і гілки в `ExceptionHandlingMiddleware` (404 / 403).
6. **Gateway:** нічого не треба, `/api/content/{**catch-all}` уже покриває новий шлях.
7. **Фронтенд:** метод у `ContentService`, кнопка в `content.html`, `toastr.success(...)`.

## Відомі обмеження

| Обмеження | Наслідок |
|---|---|
| Немає тестів | Зміни перевіряються вручну (Swagger, `.http`, браузер) |
| Немає пагінації в `GET /api/content` | При великій кількості записів відповідь росте без меж |
| Немає редагування й видалення контенту | |
| Немає refresh-токенів | Через 24 години треба входити знову |
| Однаковий `Jwt:Secret` в обох сервісах (HS256) | Будь-який сервіс із секретом може випускати токени. Альтернатива — RS256: UserService підписує приватним ключем, інші перевіряють публічним |
| Налаштування JWT продубльовані в обох `Program.cs` | При зміні треба правити в двох місцях |
| Код `ValidationBehavior` і middleware дублюється між сервісами | Свідомо: мікросервіси не ділять код. Можна винести в NuGet-пакет |
| Немає CORS / HTTPS / Docker-образів сервісів | Розраховано на локальну розробку |
| MediatR 15 вимагає ліцензійний ключ | У логах попередження `LuckyPennySoftware.MediatR.License`. Для комерційного використання потрібен ключ або MediatR 12.x |
| Токен у `localStorage` | Доступний для JS, а отже вразливий до XSS. Для продакшену краще httpOnly cookie |
| Старі записи контенту з довільним `authorId` | Створені до виправлення, не відображаються як «моє» |

## Що можна покращити

Приблизно в порядку користі:

1. **Тести:** unit-тести обробників і валідаторів (xUnit + підмінені репозиторії), інтеграційні — через `WebApplicationFactory` + Testcontainers.
2. **`GET /api/content/{id}`**, редагування та видалення з перевіркою автора.
3. **Пагінація** (`?page=&pageSize=`) та сортування на сервері.
4. **Docker Compose** для всієї системи однією командою.
5. **Автоматичні міграції** в Development (`db.Database.MigrateAsync()` при старті).
6. **ProblemDetails** (RFC 7807) замість власного формату помилок.
7. **RS256** замість HS256 і перевірка JWT у gateway.
8. **Health checks** (`/health`) для сервісів і баз.
