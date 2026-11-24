export interface ContentItem {
  id: string;
  type: 'article' | 'video' | 'unknown';
  title: string;
  authorId: string;
  createdAt: string;
  body: string | null;
  url: string | null;
  durationSeconds: number | null;
}

export interface CreateArticle {
  title: string;
  body: string;
}

export interface CreateVideo {
  title: string;
  url: string;
  durationSeconds: number;
}

export interface RegisterRequest {
  login: string;
  password: string;
  fullName: string;
}

export interface LoginRequest {
  login: string;
  password: string;
}
