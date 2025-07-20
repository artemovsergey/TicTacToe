import {Component, computed, inject} from '@angular/core';
import {RouterModule, RouterOutlet } from '@angular/router';
import { GameService } from '../services/game.service';
import { Player } from '../models/game';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'TicTacToe';
  gameService = inject(GameService)

  currentPlayer = computed(() => {return this.gameService.currentGame()?.currentMove === Player.X ? "X" : "O"});

}
