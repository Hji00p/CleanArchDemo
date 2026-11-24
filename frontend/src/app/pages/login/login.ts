import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html'
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  protected readonly loading = signal(false);
  protected readonly form = inject(NonNullableFormBuilder).group({
    login: ['', Validators.required],
    password: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => {
        this.toastr.success(`Вітаю, ${this.auth.userName()}!`, 'Вхід виконано');
        this.router.navigate(['/content']);
      },
      error: () => this.loading.set(false)
    });
  }
}
