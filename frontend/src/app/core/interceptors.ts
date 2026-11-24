import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/** Додає JWT до кожного запиту, якщо користувач залогінений. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token();
  return next(token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req);
};

/** Показує помилки API через toastr; на 401 із захищених запитів розлогінює. */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const body = err.error as { error?: string; errors?: Record<string, string[]> } | null;

      if (err.status === 0) {
        toastr.error('Сервер недоступний. Перевір, що gateway і сервіси запущені.', 'Немає звʼязку');
      } else if (err.status === 400 && body?.errors) {
        const messages = Object.values(body.errors).flat();
        toastr.warning(messages.join('<br>'), body.error ?? 'Помилка валідації', { enableHtml: true });
      } else if (err.status === 401 && !req.url.includes('/api/users/login')) {
        auth.logout();
        toastr.info('Сесія закінчилась, увійди знову.', 'Потрібен вхід');
        router.navigate(['/login']);
      } else {
        toastr.error(body?.error ?? `Помилка ${err.status}`, 'Помилка');
      }

      return throwError(() => err);
    })
  );
};
