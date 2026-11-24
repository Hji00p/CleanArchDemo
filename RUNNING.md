# Як запустити і перевірити

← [README](README.md) · повна документація в [docs/](docs/architecture.md)

## Демо-акаунт (лише для локальної розробки)

| Логін | Пароль |
|---|---|
| `demo` | `Demo-69b72dfc` |

Або зареєструй свій акаунт на сторінці «Реєстрація».

## Порядок запуску

### 1. Бази даних

| Що | Де | Як перевірити |
|---|---|---|
| PostgreSQL (UserService) | служба Windows `postgresql-x64-16`/`-18`, порт **5433** | стартує сама разом з Windows |
| MongoDB (ContentService) | Docker-контейнер `contentplatform-mongo`, порт **27017** | Docker Desktop має бути запущений |

Якщо Mongo не працює:

```powershell
docker start contentplatform-mongo
```

### 2. Бекенд (три сервіси)

**Через Visual Studio:** відкрий `CleanArchDemo.slnx`, у списку запуску вибери профіль **New Profile**
(він стартує UserService, ContentService і ApiGateway разом) і натисни F5.

**Або через термінал:** три окремі вкладки, з кореня репозиторію:

```powershell
dotnet run --project src/UserService/UserService.API --launch-profile http
```

```powershell
dotnet run --project src/ContentService/ContentService.API --launch-profile http
```

```powershell
dotnet run --project src/ApiGateway --launch-profile http
```

| Сервіс | Адреса | Swagger |
|---|---|---|
| ApiGateway | http://localhost:5161 | — |
| UserService | http://localhost:5184 | http://localhost:5184/swagger |
| ContentService | http://localhost:5137 | http://localhost:5137/swagger |

### 3. Фронтенд

```powershell
cd frontend
npm install   # лише перший раз
npm start
```

Відкрий http://localhost:4200

## Що перевірити

1. **Неправильний вхід** — введи будь-який логін і неправильний пароль → червоний toast «Невірний логін або пароль».
2. **Вхід** — `demo` + пароль з таблиці вгорі → зелений toast і сторінка «Контент».
3. **Стаття** — заповни заголовок і текст → «Статтю опубліковано», запис зʼявився в списку з позначкою «моє».
4. **Відео** — вкладка «Відео», посилання `https://...`, тривалість у секундах → «Відео додано».
5. **Валідація** — спробуй відправити порожню форму → підказки під полями.
6. **Фільтр «Моє»** — лишаються тільки твої записи.
7. **Вихід** → toast «Ти вийшов з акаунта», сторінка входу.
8. **Захист сторінок** — без входу відкрий http://localhost:4200/content → перекине на вхід.
9. **Сервер недоступний** — зупини ApiGateway і спробуй увійти → toast «Немає звʼязку».

Без фронтенду те саме можна прогнати через `src/ApiGateway/ApiGateway.http` (Visual Studio або VS Code з REST Client).
