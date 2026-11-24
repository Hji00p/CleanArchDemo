import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Content } from './pages/content/content';

export const routes: Routes = [
  { path: 'login', component: Login, canActivate: [guestGuard], title: 'Вхід' },
  { path: 'register', component: Register, canActivate: [guestGuard], title: 'Реєстрація' },
  { path: 'content', component: Content, canActivate: [authGuard], title: 'Контент' },
  { path: '', pathMatch: 'full', redirectTo: 'content' },
  { path: '**', redirectTo: 'content' }
];
