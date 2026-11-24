import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  logout(): void {
    this.auth.logout();
    this.toastr.info('Ти вийшов з акаунта');
    this.router.navigate(['/login']);
  }
}
