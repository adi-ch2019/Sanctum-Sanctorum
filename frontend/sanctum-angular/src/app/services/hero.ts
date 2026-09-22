import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Hero {
  id: string;
  name: string;
  power: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class HeroService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/heroes';

  list(): Observable<Hero[]> {
    return this.http.get<Hero[]>(this.baseUrl);
  }

  create(hero: { name: string; power: string }): Observable<Hero> {
    return this.http.post<Hero>(this.baseUrl, hero);
  }
}