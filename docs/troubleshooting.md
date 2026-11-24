# Якщо щось не працює

← [README](../README.md) · [Розробка](development.md)

Симптом → причина → що робити. Згруповано за місцем, де ви бачите проблему.

---

## Під час запуску сервісу

### `DirectoryNotFoundException: ...\ContentService.API\wwwroot\`

**Причина.** У `bin` лишився маніфест статичних файлів (`*.staticwebassets.runtime.json`) від старої збірки,
коли в проєкті ще була папка `wwwroot`. У Development ASP.NET читає його і намагається відкрити неіснуючу папку.

**Рішення.** Visual Studio: **Build → Clean Solution**, потім **Rebuild Solution**. Або вручну:

```powershell
Remove-Item -Recurse -Force src\ContentService\ContentService.API\bin, src\ContentService\ContentService.API\obj
```

### `Failed to bind to address http://127.0.0.1:5184: address already in use`

**Причина.** Порт зайнятий іншою копією сервісу (часто лишається після зупинки дебагу).

**Рішення.** Знайти й зупинити процес:

```powershell
Get-NetTCPConnection -LocalPort 5184 -State Listen | ForEach-Object { Get-Process -Id $_.OwningProcess }
```

```powershell
Stop-Process -Id <PID>
```

### Збірка: `The process cannot access the file '...\UserService.API.exe' because it is being used by another process`

**Причина.** Сервіс запущений і тримає свій `.exe`.

**Рішення.** Зупиніть запуск у Visual Studio (Shift+F5) і зберіть знову.

### Попередження `LuckyPennySoftware.MediatR.License`

**Причина.** MediatR з версії 13 комерційний і просить ліцензійний ключ.

**Рішення.** Для навчання можна ігнорувати, на роботу це не впливає.
Інакше — ключ від LuckyPenny Software або `MediatR` версії 12.x (остання безкоштовна).

---

## Під час входу і реєстрації

### Вхід завжди дає 500 «Внутрішня помилка сервера»

Відкрийте вікно консолі UserService: там повний стек.

| Що в лозі | Причина | Рішення |
|---|---|---|
| `IDX10720` / `key size must be greater than: '256' bits` | `Jwt:Secret` коротший за 32 символи | задайте довший секрет |
| `ArgumentException` про ключ / порожній ключ | секрет не задано, береться `""` з `appsettings.json` | `dotnet user-secrets set "Jwt:Secret" ...` |
| `Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:5433` | PostgreSQL не запущений | запустіть службу `postgresql-x64-16` |
| `28P01: password authentication failed` | неправильний пароль у рядку підключення | перевірте `ConnectionStrings:Default` |
| `42P01: relation "Users" does not exist` | не застосовано міграції | `dotnet ef database update ...`, див. [Розробка](development.md#міграції-ef-core-userservice) |

### Реєстрація дає 409, хоча такого користувача «немає»

Логін порівнюється **з урахуванням регістру й пробілів**: `Demo` і `demo` — різні логіни,
але `demo` вже зайнятий. Перевірити наявних:

```sql
SELECT "Login" FROM "Users";
```

---

## Під час роботи з контентом

### 401 на всі запити до `/api/content`, хоча я увійшов

| Причина | Як перевірити | Рішення |
|---|---|---|
| Токен прострочений (24 год) | вийдіть і увійдіть знову | — |
| `Jwt:Secret` / `Issuer` / `Audience` у ContentService відрізняються від UserService | `dotnet user-secrets list` в обох `.API` | зробіть однаковими |
| У Swagger вставлено `Bearer eyJ...` | — | вставляйте **лише** `eyJ...`, без слова `Bearer` |
| Токен скопійовано з лапками | — | без `"` |

Щоб побачити точну причину, увімкніть детальне логування автентифікації в
`src/ContentService/ContentService.API/appsettings.Development.json`:

```json
"Logging": {
  "LogLevel": {
    "Microsoft.AspNetCore.Authentication": "Debug"
  }
}
```

У лозі зʼявиться, наприклад, `IDX10223: Lifetime validation failed` (прострочений)
або `IDX10511: Signature validation failed` (інший секрет).

### `GET /api/content` повертає 500

Найчастіше Mongo недоступний: у лозі `TimeoutException ... A timeout occurred after 30000ms selecting a server`.

```powershell
docker start contentplatform-mongo
```

Запит при цьому висить **30 секунд**, перш ніж впасти: стільки драйвер чекає на сервер за замовчуванням.

### Мій старий запис не позначається як «моє»

Записи, створені до того, як `authorId` почали брати з токена, мають довільний `authorId`.
Їх можна виправити в Mongo вручну:

```powershell
docker exec -it contentplatform-mongo mongosh ContentServiceDb
```

```js
db.content.find({}, { title: 1, authorId: 1 })
db.content.updateOne({ _id: "<id запису>" }, { $set: { authorId: "<ваш Guid з таблиці Users>" } })
```

---

## Фронтенд

### Toast «Немає звʼязку» при будь-якій дії

Фронтенд не може достукатися до gateway.

1. Чи запущений ApiGateway на `:5161`? Перевірте http://localhost:5161/api/content: має бути 401, а не помилка зʼєднання.
2. Чи запущено фронтенд через `npm start` (а не, наприклад, відкриттям `dist/index.html`)? Proxy працює лише в `ng serve`.
3. Після зміни `proxy.conf.json` перезапустіть `npm start`.

### Порожня сторінка або помилка після `npm start`

- `npm install` не виконано, або виконано в іншій папці. Його треба запускати саме в `frontend/`.
- Помилка компіляції: дивіться вивід `npm start` у терміналі.

### Мене викидає на сторінку входу одразу після входу

`errorInterceptor` розлогінює при будь-якій 401 з API. Якщо ContentService відхиляє токен
(див. [401 на всі запити](#401-на-всі-запити-до-apicontent-хоча-я-увійшов)), ви одразу опинитесь на `/login`.

### Шаблон не оновлюється після зміни даних

Застосунок **zoneless**: Angular оновлює шаблон лише коли змінюється signal (або на подію в шаблоні).
Звичайне поле класу, змінене всередині `subscribe`, екран не оновить.

```ts
// ❌ не оновить екран
this.items = data;

// ✅
this.items.set(data);
```

### `Node.js version v25 detected. Odd numbered Node.js versions will not enter LTS`

Лише попередження. Для стабільності встановіть Node 24 LTS.

---

## Корисні команди

| Що | Команда |
|---|---|
| Хто слухає порт | `Get-NetTCPConnection -LocalPort 5161 -State Listen` |
| Секрети сервісу | `cd src/UserService/UserService.API; dotnet user-secrets list` |
| Контейнери | `docker ps -a` |
| Документи в Mongo | `docker exec -it contentplatform-mongo mongosh ContentServiceDb --eval "db.content.find()"` |
| Застосувати міграції | `dotnet ef database update --project src/UserService/UserService.Infrastructure --startup-project src/UserService/UserService.API` |
| Чиста збірка | `dotnet clean CleanArchDemo.slnx; dotnet build CleanArchDemo.slnx` |
