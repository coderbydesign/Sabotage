using System;
using Sabotage.Views;
using Sabotage.ViewModels;
using Avalonia.Threading;

namespace Sabotage {
    public class GameLogic {
        public static int[,] board = new int[10, 10];
        public static int xHit;
        public static int yHit;

        public static void ReceiveFire(int x, int y) {
            
            board[x, y] = 1;
            xHit = x;
            yHit = y;
            Action receiveFireUI = delegate() {MainWindow.ReceiveFire();};
            Dispatcher.UIThread.InvokeAsync(receiveFireUI);
        }
    }
}