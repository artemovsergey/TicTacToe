export interface Game {
  id: string;
  status: StatusGame;
  board: string[][] | null;
  result: ResultGame;
  createdAt: string;
  currentMove: Player;
  currentStep: number;
}

export enum StatusGame {
  Active = "Active",
  Complete = "Complete"
}

export enum ResultGame {
  None = "None",
  XWon = "XWon",
  OWon = "OWon",
  Draw = "Draw"
}

export enum Player {
  X = 'X',
  O = 'O'
}
