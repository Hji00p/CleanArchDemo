# Довідник API

← [README](../README.md)

Базова адреса — **gateway**: `http://localhost:5161`. Усі тіла запитів і відповідей у JSON, поля в camelCase.

| Метод | Шлях | Авторизація | Що робить |
|---|---|---|---|
| POST | [`/api/users/register`](#post-apiusersregister) | — | Реєстрація |
| POST | [`/api/users/login`](#post-apiuserslogin) | — | Вхід, отримання JWT |
| GET | [`/api/content`](#get-apicontent) | Bearer | Увесь контент |
| POST | [`/api/content/article`](#post-apicontentarticle) | Bearer | Створити статтю |
| POST | [`/api/content/video`](#post-apicontentvideo) | Bearer | Створити відео |

Авторизація — заголовок:

```
Authorization: Bearer <token>
```

---

## POST /api/users/register

**Запит**

```json
{
  "login": "john",
  "password": "Password123",
  "fullName": "John Doe"
}
```

| Поле | Обовʼязкове | Правило |
|---|---|---|
| `login` | так | 3–50 символів, унікальний |
| `password` | так | 8–100 символів |
| `fullName` | так | до 200 символів |

**Відповіді**

| Код | Тіло |
|---|---|
| 200 | `{ "userId": "75939ef8-830f-46c4-b811-2932acb84c9f" }` |
| 400 | помилки валідації, див. [формат](#формат-помилок) |
| 409 | `{ "error": "Користувач з таким логіном вже існує" }` |

Реєстрація **не повертає токен**, після неї потрібен `login`. Фронтенд робить це автоматично.

---

## POST /api/users/login

**Запит**

```json
{
  "login": "john",
  "password": "Password123"
}
```

**Відповіді**

| Код | Тіло |
|---|---|
| 200 | `{ "token": "eyJhbGciOiJIUzI1NiIs..." }` |
| 400 | порожній логін або пароль |
| 401 | `{ "error": "Невірний логін або пароль" }` |

Токен діє **24 години**. Вміст payload:

```json
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "75939ef8-…",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "john",
  "exp": 1795600960,
  "iss": "UserService",
  "aud": "MyApp"
}
```

---

## GET /api/content

Повертає **весь** контент усіх користувачів (без пагінації).

**Відповідь 200**

```json
[
  {
    "id": "64c0ba33-de1b-4446-88b9-81cad6a6dc5b",
    "type": "article",
    "title": "Моя стаття",
    "authorId": "75939ef8-830f-46c4-b811-2932acb84c9f",
    "createdAt": "2026-11-24T10:02:40.999Z",
    "body": "Текст статті",
    "url": null,
    "durationSeconds": null
  },
  {
    "id": "7ee55ad7-ded4-44ae-8678-7d6cb5b647fc",
    "type": "video",
    "title": "Моє відео",
    "authorId": "75939ef8-830f-46c4-b811-2932acb84c9f",
    "createdAt": "2026-11-24T10:02:41.358Z",
    "body": null,
    "url": "https://example.com/v.mp4",
    "durationSeconds": 120
  }
]
```

| Поле | Опис |
|---|---|
| `type` | `article`, `video` або `unknown` |
| `body` | лише для `article` |
| `url`, `durationSeconds` | лише для `video` |

Порядок не гарантований, фронтенд сортує за `createdAt`.

**Помилки:** `401` без токена або з простроченим токеном (тіло порожнє).

---

## POST /api/content/article

**Запит**

```json
{
  "title": "Моя стаття",
  "body": "Текст статті"
}
```

| Поле | Правило |
|---|---|
| `title` | не порожній, до 200 символів |
| `body` | не порожній |

Автор визначається **з токена**. Поле `authorId` у тілі ігнорується.

**Відповіді:** `200 { "id": "…" }`, `400`, `401`.

---

## POST /api/content/video

**Запит**

```json
{
  "title": "Моє відео",
  "url": "https://example.com/v.mp4",
  "durationSeconds": 120
}
```

| Поле | Правило |
|---|---|
| `title` | не порожній, до 200 символів |
| `url` | абсолютний `http://` або `https://` |
| `durationSeconds` | ціле, більше 0 |

**Відповіді:** `200 { "id": "…" }`, `400`, `401`.

---

## Формат помилок

**400 — валідація.** Ключі `errors` — назви властивостей команди (з великої літери):

```json
{
  "error": "Помилка валідації",
  "errors": {
    "Title": ["'Title' не може бути порожнім."],
    "Url": ["Url має бути абсолютним http/https посиланням"],
    "DurationSeconds": ["'Duration Seconds' має бути більшим за '0'."]
  }
}
```

Повідомлення FluentValidation українською, бо мова береться з культури системи.

**401 / 409 / 500:**

```json
{ "error": "Текст помилки" }
```

| Код | Коли |
|---|---|
| 400 | не пройшла валідація |
| 401 | невірний логін/пароль; немає, недійсний або прострочений токен |
| 409 | логін зайнятий |
| 500 | непередбачена помилка; текст завжди `"Внутрішня помилка сервера"`, деталі в логах сервісу |

## Готові запити

- [`src/ApiGateway/ApiGateway.http`](../src/ApiGateway/ApiGateway.http) — увесь сценарій через gateway (Visual Studio / VS Code REST Client).
- [`src/UserService/UserService.API/UserService.API.http`](../src/UserService/UserService.API/UserService.API.http) — реєстрація та вхід напряму.
- Swagger кожного сервісу: http://localhost:5184/swagger, http://localhost:5137/swagger.
