using System;
using Sabotage.Views;
using Sabotage.ViewModels;
using Avalonia.Threading;

namespace Sabotage {
    public class GameLogic {
        private const int boardSize = 10;
        public static Tile[,] board = new Tile[boardSize, boardSize];
        public static int xHit;
        public static int yHit;
        public static bool isOccupiedHit = false;

        public static void InitializeBoard() {
            for (int x = 0; x < boardSize; x++) {
                for (int y = 0; y < boardSize; y++) {
                    board[x, y] = new Tile();
                }
            }

            board[0,0].isOccupied = true;
        }

        public static void ReceiveFire(int x, int y) {
            
            board[x, y].isHit = true;
            isOccupiedHit = board[x, y].isOccupied;
            xHit = x;
            yHit = y;
            Action receiveFireUI = delegate() {MainWindow.ReceiveFire();};
            Dispatcher.UIThread.InvokeAsync(receiveFireUI);
            ClientSend.ConfirmHit(xHit, yHit, isOccupiedHit);
        }

        public class Tile {
            public bool isHit;
            public bool isOccupied;

            public Tile() {
                isHit = false;
                isOccupied = false;
            }
        }
    }
}