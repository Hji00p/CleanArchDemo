# CleanArchDemo — frontend

Angular 21 client for CleanArchDemo with ngx-toastr notifications.

```bash
npm install
npm start      # http://localhost:4200
```

Requests to `/api/*` are proxied to the gateway at `http://localhost:5161` (`proxy.conf.json`), so the backend must be running.

**Features:** login and registration, content list with All/Mine filter, create articles and videos, toast notifications for success and API errors, automatic logout on 401.

```
src/app/
  core/    auth and content services, HTTP interceptors, route guards, models
  pages/   login, register, content
```
