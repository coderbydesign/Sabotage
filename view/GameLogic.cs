using System;
using System.Collections.Generic;
using Sabotage.Views;
using Sabotage.ViewModels;
using Avalonia.Threading;

namespace Sabotage {
    public class GameLogic {
        private const int boardSize = 10;
        public static Tile[,] board = new Tile[boardSize, boardSize];
        public static Dictionary<string, Tile[]> services = new Dictionary<string, Tile[]>();
        public static int xReceived;
        public static int yReceived;
        public static bool isOccupiedTile = false;

        public static int xTarget;
        public static int yTarget;
        public static bool isOccupiedTarget;

        public static void InitializeBoard() {
            for (int x = 0; x < boardSize; x++) {
                for (int y = 0; y < boardSize; y++) {
                    board[x, y] = new Tile(x, y);
                }
            }

            PlaceServices();
        }

        public static void ReceiveFire(int x, int y) {
            Tile target = board[x, y];
            target.isHit = true;
            isOccupiedTile = target.isOccupied;
            bool serviceSunk = false;

            if (isOccupiedTile) {
                serviceSunk = true;
                foreach(Tile tile in services[target.serviceName]) {
                    if(!tile.isHit) {
                        serviceSunk = false;
                        break;
                    }
                }
            }

            if(serviceSunk) {
                Console.WriteLine($"{target.serviceName} has been sunk!");
            }

            xReceived = x;
            yReceived = y;
            Action receiveFireUI = delegate() {MainWindow.ReceiveFire();};
            Dispatcher.UIThread.InvokeAsync(receiveFireUI);
            ClientSend.ConfirmHit(xReceived, yReceived, isOccupiedTile);
        }

        public static void TargetHit(int x, int y, bool hitService) {
            xTarget = x;
            yTarget = y;
            isOccupiedTarget = hitService;
            Action targetHitUpdateUI = delegate() {MainWindow.UpdateTarget();};
            Dispatcher.UIThread.InvokeAsync(targetHitUpdateUI);
        }

        public static void PlaceServices() {
            // RBAC is a vertical line of length 3
            string serviceName = "RBAC";
            services.Add(serviceName, new Tile[3]{board[1,2].AddService(serviceName), 
                                                  board[1,3].AddService(serviceName), 
                                                  board[1,4].AddService(serviceName)});

            // 3Scale is a horizontal line of length 4
            serviceName = "3Scale";
            services.Add(serviceName, new Tile[4]{board[2,2].AddService(serviceName), 
                                                  board[3,2].AddService(serviceName), 
                                                  board[4,2].AddService(serviceName),
                                                  board[5,2].AddService(serviceName)});
        }

        public class Tile {
            public bool isHit;
            public bool isOccupied;
            public int x;
            public int y;
            public string serviceName;

            public Tile(int _x, int _y) {
                isHit = false;
                isOccupied = false;
                x = _x;
                y = _y;
            }

            public Tile AddService(string _serviceName) {
                serviceName = _serviceName;
                isOccupied = true;
                return this;
            }
        }
    }
}