# CleanArchDemo — frontend

Простий клієнт на Angular 21 + ngx-toastr для UserService і ContentService.

## Запуск

1. Запусти ApiGateway (`http://localhost:5161`), UserService і ContentService.
2. У цій папці:

   ```
   npm install
   npm start
   ```

3. Відкрий http://localhost:4200

Запити на `/api/*` dev-server проксує на gateway (`proxy.conf.json`), тому CORS налаштовувати не треба.

## Що є

- **Вхід / Реєстрація.** Після реєстрації користувач одразу входить. JWT зберігається в `localStorage`.
- **Контент.** Список статей і відео з фільтром «Усе / Моє», форма додавання статті або відео.
- **Toastr.** Успіх, помилки валідації з бекенду (список полів), 401 (автоматичний вихід), сервер недоступний.

## Структура

```
src/app/
  core/
    auth.service.ts      логін, реєстрація, токен, імʼя користувача з JWT
    content.service.ts   запити до /api/content
    interceptors.ts      додавання Bearer-токена; показ помилок через toastr
    guards.ts            authGuard / guestGuard
    models.ts            типи DTO
  pages/
    login/  register/  content/
```
