import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { GameService } from '../../services/game.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-game-form',
  templateUrl: './game-form.html',
  styleUrls: ['./game-form.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class NewGameFormComponent {
  private fb = inject(FormBuilder);
  private gameService = inject(GameService);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  gameForm = this.fb.group({
    size: [3, [Validators.required, Validators.min(3), Validators.max(10)]],
    line_to_win: [3, [Validators.required, Validators.min(3)]],
    chance: [1, [Validators.min(0), Validators.max(100)]],
    step: [1, [Validators.min(0)]]
  });

  onSubmit(): void {
    if (this.gameForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    const formValue = this.gameForm.value;
    const gameOptions = {
      size: formValue.size!,
      line_to_win: formValue.line_to_win!,
      chance: formValue.chance || 0,
      step: formValue.step || 0
    };

    this.gameService.createGame(gameOptions).subscribe({
      next: (game) => {
        this.loading.set(false);
        this.router.navigate(['/game', game.id]);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.message || 'Failed to create game');
      }
    });
  }
}
