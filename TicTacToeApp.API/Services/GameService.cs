using TicTacToeApp.API.Entity.Enums;

namespace TicTacToeApp.API.Services;

internal sealed class GameService
{
    internal static ResultGame CheckBoard(string?[][] arr, string p)
    {
        if ((arr[0][0] == p && arr[0][0] == arr[0][1] && arr[0][1] == arr[0][2]) ||
            (arr[1][0] == p && arr[1][0] == arr[1][1] && arr[1][1] == arr[1][2]) ||
            (arr[2][0] == p && arr[2][0] == arr[2][1] && arr[2][1] == arr[2][2]) ||
            (arr[0][0] == p && arr[0][0] == arr[1][0] && arr[1][0] == arr[2][0]) ||
            (arr[0][1] == p && arr[0][1] == arr[1][1] && arr[1][1] == arr[2][1]) ||
            (arr[0][2] == p && arr[0][2] == arr[1][2] && arr[1][2] == arr[2][2]) ||
            (arr[0][0] == p && arr[0][0] == arr[1][1] && arr[1][1] == arr[2][2]) ||
            (arr[2][0] == p && arr[2][0] == arr[1][1] && arr[1][1] == arr[0][2])
           )
        {
            switch (p)
            {
                case "X":
                    return ResultGame.WinX;
                case "O":
                    return ResultGame.WinO;
            }
        }

        var canContinueGame = arr.Any(cell => cell == null);
        if (!canContinueGame) return ResultGame.None;

        return ResultGame.Draw;
    }
    internal static ResultGame CheckBoardN(string?[][] board, string player, int lineToWin)
    {
        int size = board.Length;

        // Проверка всех возможных линий
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // Пропускаем пустые клетки
                if (board[i][j] != player)
                    continue;

                // Проверка горизонтали (→)
                if (CheckLine(board, i, j, 0, 1, player, lineToWin))
                    return GetWinResult(player);

                // Проверка вертикали (↓)
                if (CheckLine(board, i, j, 1, 0, player, lineToWin))
                    return GetWinResult(player);

                // Проверка диагонали (↘)
                if (CheckLine(board, i, j, 1, 1, player, lineToWin))
                    return GetWinResult(player);

                // Проверка диагонали (↙)
                if (CheckLine(board, i, j, 1, -1, player, lineToWin))
                    return GetWinResult(player);
            }
        }

        // Проверка на ничью (если нет пустых клеток)
        bool isDraw = !board.Any(row => row.Any(cell => cell == null));
        return isDraw ? ResultGame.Draw : ResultGame.None;
    }

    // Проверяет, есть ли K символов player в ряд, начиная с (startX, startY)
    private static bool CheckLine(string?[][] board, int startX, int startY, int dx, int dy, string player, int lineToWin)
    {
        int size = board.Length;
        int count = 0;

        for (int step = 0; step < lineToWin; step++)
        {
            int x = startX + step * dx;
            int y = startY + step * dy;

            // Выход за границы поля
            if (x < 0 || x >= size || y < 0 || y >= size)
                break;

            if (board[x][y] == player)
                count++;
            else
                break;
        }

        return count == lineToWin;
    }

    // Возвращает ResultGame.WinX или ResultGame.WinO
    private static ResultGame GetWinResult(string player)
    {
        return player == "X" ? ResultGame.WinX : ResultGame.WinO;
    }
}