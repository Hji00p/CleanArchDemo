import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ContentItem, CreateArticle, CreateVideo } from './models';

@Injectable({ providedIn: 'root' })
export class ContentService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<ContentItem[]> {
    return this.http.get<ContentItem[]>('/api/content');
  }

  createArticle(request: CreateArticle): Observable<string> {
    return this.http.post<{ id: string }>('/api/content/article', request).pipe(map(r => r.id));
  }

  createVideo(request: CreateVideo): Observable<string> {
    return this.http.post<{ id: string }>('/api/content/video', request).pipe(map(r => r.id));
  }
}
