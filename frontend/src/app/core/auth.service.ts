import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { LoginRequest, RegisterRequest } from './models';

const TOKEN_KEY = 'token';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const ID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

interface TokenPayload {
  [NAME_CLAIM]?: string;
  [ID_CLAIM]?: string;
  exp?: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenSignal = signal<string | null>(this.readValidToken());

  readonly token = this.tokenSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.tokenSignal() !== null);
  readonly payload = computed(() => {
    const token = this.tokenSignal();
    return token ? decode(token) : null;
  });
  readonly userName = computed(() => this.payload()?.[NAME_CLAIM] ?? '');
  readonly userId = computed(() => this.payload()?.[ID_CLAIM] ?? '');

  register(request: RegisterRequest): Observable<string> {
    return this.http
      .post<{ userId: string }>('/api/users/register', request)
      .pipe(map(r => r.userId));
  }

  login(request: LoginRequest): Observable<void> {
    return this.http.post<{ token: string }>('/api/users/login', request).pipe(
      tap(r => {
        localStorage.setItem(TOKEN_KEY, r.token);
        this.tokenSignal.set(r.token);
      }),
      map(() => undefined)
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.tokenSignal.set(null);
  }

  // Прострочений токен одразу викидаємо, щоб не показувати користувача як залогіненого
  private readValidToken(): string | null {
    const token = localStorage.getItem(TOKEN_KEY);
    if (!token) return null;

    const exp = decode(token)?.exp;
    if (!exp || exp * 1000 < Date.now()) {
      localStorage.removeItem(TOKEN_KEY);
      return null;
    }
    return token;
  }
}

function decode(token: string): TokenPayload | null {
  try {
    const part = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(part)
        .split('')
        .map(c => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    );
    return JSON.parse(json) as TokenPayload;
  } catch {
    return null;
  }
}
