import { Routes } from '@angular/router';
import { GameListComponent } from './game-list/game-list';
import { NewGameFormComponent } from './game-form/game-form';
import { GameBoardComponent } from './game-board/game-board';


export const routes: Routes = [
  { path: '', component: GameListComponent },
  { path: 'new-game', component: NewGameFormComponent },
  { path: 'game/:id', component: GameBoardComponent },
  { path: '**', redirectTo: '' }
];
