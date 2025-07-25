import {inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, tap } from 'rxjs';
import { Game } from '../models/game';
import { GameOption } from '../models/game-option';
import { Move } from '../models/move';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = environment.apiUrl;
  currentGame = signal<Game | null>(null);
  gamesList = signal<Game[]>([]);

  http = inject(HttpClient)

  getAllGames(): Observable<Game[]> {
    return this.http.get<Game[]>(`${this.apiUrl}/games`).pipe(
      tap(games => this.gamesList.set(games))
    );
  }

  getGameById(id: string | null): Observable<Game> {
    return this.http.get<Game>(`${this.apiUrl}/games/${id}`).pipe(
      tap(game => this.currentGame.set(game))
    );
  }

  createGame(options: GameOption): Observable<Game> {
    return this.http.post<Game>(`${this.apiUrl}/games/new`, options).pipe(
      tap(game => {
        this.currentGame.set(game);
        this.gamesList.update(games => [...games, game]);
      })
    );
  }

  makeMove(id: string, move: Move, etag: string | null): Observable<Game> {
    // const headers = new HttpHeaders({'If-Match': "null"});
    const headers = new HttpHeaders({});
    return this.http.post<Game>(`${this.apiUrl}/games/${id}/move`, move, { headers }).pipe(
      tap(game => {console.log("Ответ: ", game) ;this.currentGame.set(game)})
    );
  }

  // getCurrentGameEtag(): string | null {
  //   const game = this.currentGame();
  //   return game ? this.generateEtag(game) : null;
  // }

  // private generateEtag(game: Game): string {
  //   return btoa(JSON.stringify(game));
  // }
}
