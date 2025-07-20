import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {GameService} from '../../services/game.service';

import {CommonModule} from '@angular/common';
import {Game, Player, ResultGame, StatusGame} from '../../models/game';
import {Move} from '../../models/move';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.html',
  styleUrls: ['./game-board.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class GameBoardComponent implements OnInit{


  route = inject(ActivatedRoute)
  gameService = inject(GameService);
  currentId= this.route.snapshot.paramMap.get('id');

  ngOnInit(): void {
    this.gameService.getGameById(this.currentId).subscribe(r => {
      this.gameService.currentGame.set(r);
      console.log("Запрос:  ",r);
    })
  }

  currentGame = computed(() => { console.log("computed prop..."); return this.gameService.currentGame()?.id })
  boardSize = computed(() => {console.log("Загрузка boardSize..."); return this.gameService.currentGame()?.board?.length || 0  });
  currentPlayer = computed(() => {return this.gameService.currentGame()?.currentMove === Player.X ? "X" : "O"});
  gameStatus = computed(() => {

    console.log("Загрузка статуса игры...")
    // if (this.game() === null) return 'Loading...';
    if (this.gameService.currentGame()?.status === StatusGame.Complete) {
      switch (this.gameService.currentGame()?.result) {
        case ResultGame.XWon:
          return 'X won!';
        case ResultGame.OWon:
          return 'O won!';
        case ResultGame.Draw:
          return 'Draw!';
        default:
          return 'Game Over';
      }
    }
    return `Current player: ${this.currentPlayer()}`;
  });


  makeMove(x: number, y: number) {
    // const game = this.gameService.currentGame;
    if (this.gameService.currentGame()?.status === StatusGame.Complete) return;

    const move: Move = {
      p: this.gameService.currentGame()!.currentMove,
      x,
      y
    };

    // const etag = this.gameService.getCurrentGameEtag();
    this.gameService.makeMove(this.currentId!, move, null).subscribe(r => {console.log(r.board, r.id)});
  }

  getCellValue(x: number, y: number): string {
    const board = this.gameService.currentGame()?.board;
    if (!board || x >= board.length || y >= board.length) return '';
    return board[x][y] || '';
  }

  trackByIndex(index: number): number {
    return index;
  }

  protected readonly ResultGame = ResultGame;
}
