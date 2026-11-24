import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { switchMap } from 'rxjs';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html'
})
export class Register {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  protected readonly loading = signal(false);
  // Ті самі правила, що й на бекенді (RegisterUserCommandValidator)
  protected readonly form = inject(NonNullableFormBuilder).group({
    login: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
    fullName: ['', [Validators.required, Validators.maxLength(200)]]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { login, password } = this.form.getRawValue();
    this.loading.set(true);

    // Після реєстрації одразу логінимо, щоб не вводити дані двічі
    this.auth
      .register(this.form.getRawValue())
      .pipe(switchMap(() => this.auth.login({ login, password })))
      .subscribe({
        next: () => {
          this.toastr.success('Акаунт створено, ти вже увійшов', 'Готово');
          this.router.navigate(['/content']);
        },
        error: () => this.loading.set(false)
      });
  }
}
