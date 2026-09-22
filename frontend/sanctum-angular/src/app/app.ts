import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { HeroService, Hero } from './services/hero';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent implements OnInit{
  private heroService = inject(HeroService);

  heroes = signal<Hero[]>([]);
  loading = signal(false);
  newHero = { name: '', power: '' };

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.heroService.list().subscribe({
      next: (data) => this.heroes.set(data),
      error: (err) => console.error('Failed to load heroes', err)
    });
  }

  createHero(): void {
    if (!this.newHero.name.trim() || !this.newHero.power.trim()) return;

    this.loading.set(true);
    this.heroService.create(this.newHero).subscribe({
      next: () => {
        this.newHero = { name: '', power: '' };
        this.loading.set(false);
        this.refresh();
      },
      error: (err: unknown) => {
        console.error('Failed to create hero', err);
        this.loading.set(false);
      }
    });
  }
}