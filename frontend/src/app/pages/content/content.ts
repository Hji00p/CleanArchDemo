import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/auth.service';
import { ContentService } from '../../core/content.service';
import { ContentItem } from '../../core/models';

type Kind = 'article' | 'video';
type Filter = 'all' | 'mine';

const HTTP_URL = /^https?:\/\/\S+$/i;

@Component({
  selector: 'app-content',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './content.html',
  styleUrl: './content.css'
})
export class Content implements OnInit {
  private readonly contentService = inject(ContentService);
  private readonly auth = inject(AuthService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly items = signal<ContentItem[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly kind = signal<Kind>('article');
  protected readonly filter = signal<Filter>('all');

  protected readonly visible = computed(() => {
    const list = [...this.items()].sort((a, b) => b.createdAt.localeCompare(a.createdAt));
    return this.filter() === 'mine' ? list.filter(i => i.authorId === this.auth.userId()) : list;
  });

  protected readonly articleForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    body: ['', Validators.required]
  });

  protected readonly videoForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    url: ['', [Validators.required, Validators.pattern(HTTP_URL)]],
    durationSeconds: [60, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.load();
  }

  isMine(item: ContentItem): boolean {
    return item.authorId === this.auth.userId();
  }

  formatDuration(seconds: number | null): string {
    if (!seconds) return '';
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  load(): void {
    this.loading.set(true);
    this.contentService.getAll().subscribe({
      next: items => {
        this.items.set(items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  submit(): void {
    const form = this.kind() === 'article' ? this.articleForm : this.videoForm;
    if (form.invalid) {
      form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const request$ =
      this.kind() === 'article'
        ? this.contentService.createArticle(this.articleForm.getRawValue())
        : this.contentService.createVideo(this.videoForm.getRawValue());

    request$.subscribe({
      next: () => {
        this.toastr.success(this.kind() === 'article' ? 'Статтю опубліковано' : 'Відео додано');
        form.reset();
        this.saving.set(false);
        this.load();
      },
      error: () => this.saving.set(false)
    });
  }
}
