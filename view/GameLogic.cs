using System;
using Sabotage.Views;
using Sabotage.ViewModels;
using Avalonia.Threading;

namespace Sabotage {
    public class GameLogic {
        private const int boardSize = 10;
        public static Tile[,] board = new Tile[boardSize, boardSize];
        public static int xReceived;
        public static int yReceived;
        public static bool isOccupiedReceived = false;

        public static int xTarget;
        public static int yTarget;
        public static bool isOccupiedTarget;

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
            isOccupiedReceived = board[x, y].isOccupied;
            xReceived = x;
            yReceived = y;
            Action receiveFireUI = delegate() {MainWindow.ReceiveFire();};
            Dispatcher.UIThread.InvokeAsync(receiveFireUI);
            ClientSend.ConfirmHit(xReceived, yReceived, isOccupiedReceived);
        }

        public static void TargetHit(int x, int y, bool hitShip) {
            xTarget = x;
            yTarget = y;
            isOccupiedTarget = hitShip;
            Action targetHitUpdateUI = delegate() {MainWindow.UpdateTarget();};
            Dispatcher.UIThread.InvokeAsync(targetHitUpdateUI);
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