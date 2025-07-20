import { Component, inject, signal } from '@angular/core';
import { GameService } from '../../services/game.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-game-list',
  templateUrl: './game-list.html',
  styleUrls: ['./game-list.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class GameListComponent {
  private gameService = inject(GameService);

  games = this.gameService.gamesList;
  loading = signal(false);

  ngOnInit() {
    this.loading.set(true);
    this.gameService.getAllGames().subscribe({
      next: () => this.loading.set(false),
      error: () => this.loading.set(false)
    });
  }
}
