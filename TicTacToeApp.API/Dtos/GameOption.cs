namespace TicTacToeApp.API.Dtos;

public record GameOption(
    int size,
    int line_to_win,
    int chance,
    int step);