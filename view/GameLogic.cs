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

        public static bool myTurn = false;
        private static Random rng = new Random();

        public static void InitializeBoard() {
            for (int x = 0; x < boardSize; x++) {
                for (int y = 0; y < boardSize; y++) {
                    board[x, y] = new Tile(x, y);
                }
            }

            PlaceServices();
        }

        // They attacked us
        public static void ReceiveFire(int x, int y) {
            myTurn = true;

            Tile target = board[x, y];
            target.isHit = true;
            isOccupiedTile = target.isOccupied;
            bool serviceSunk = false;
            bool everyServiceSunk = false;

            if (isOccupiedTile) {
                serviceSunk = true;
                foreach(Tile tile in services[target.serviceName]) {
                    if(!tile.isHit) {
                        serviceSunk = false;
                        break;
                    }
                }

                if(serviceSunk) {
                    everyServiceSunk = true;
                    foreach ( KeyValuePair<string, Tile[]> kvp in services ) {
                        foreach (Tile tile in kvp.Value) {
                            if (!tile.isHit) {
                                everyServiceSunk = false;
                                break;
                            }
                        }

                        if(!everyServiceSunk) break;
                    }
                }
            }

            if(serviceSunk) {
                Console.WriteLine($"They sunk {target.serviceName}!");
                ClientSend.ConfirmServiceSunk(target.serviceName);
            }

            if(everyServiceSunk) {
                Console.WriteLine("They sunk all of our services!");
                ClientSend.ConfirmAllServicesSunk();
            }

            xReceived = x;
            yReceived = y;
            Action receiveFireUI = delegate() {MainWindow.ReceiveFire();};
            Dispatcher.UIThread.InvokeAsync(receiveFireUI);

            Action updateTurnTracker = delegate() {MainWindow.UpdateTurnTracker();};
            Dispatcher.UIThread.InvokeAsync(updateTurnTracker);

            ClientSend.ConfirmHit(xReceived, yReceived, isOccupiedTile);
        }

        public static void Fire(int x, int y) {
            GameLogic.myTurn = false;

            ClientSend.Fire(x, y);

            Action updateTurnTracker = delegate() {MainWindow.UpdateTurnTracker();};
            Dispatcher.UIThread.InvokeAsync(updateTurnTracker);
        }

        // We are attacking them
        public static void TargetHit(int x, int y, bool hitService) {
            xTarget = x;
            yTarget = y;
            isOccupiedTarget = hitService;
            Action targetHitUpdateUI = delegate() {MainWindow.UpdateTarget();};
            Dispatcher.UIThread.InvokeAsync(targetHitUpdateUI);
        }

        public static void PlaceServices() {
            // RBAC is a vertical line of length 3
            //PlaceService("RBAC", 3, false);

            // 3Scale is a horizontal line of length 4
            //PlaceService("3Scale", 4, true);

            //PlaceService("app-sre", 5, true);

            //PlaceService("Kafka", 4, false);

            //PlaceService("Clowder", 3, true);

            //PlaceService("Notifications", 2, true);

            PlaceService("Backoffice Proxy", 2, true);
            
        }

        public static void PlaceService(string serviceName, int length, bool isHorizontal) {
            bool placed = false;
            Tile[] possiblePlacement = new Tile[length];

            if(isHorizontal) {

                while(!placed) {
                    // clear last placement attempt
                    possiblePlacement = new Tile[length];
                    // assume we can place the service
                    placed = true;
                    // choose a starting point
                    int xStart = rng.Next(boardSize - length);
                    int yStart = rng.Next(boardSize);
                    Console.WriteLine($"Start trying ({xStart}, {yStart})"); 
                    for (int x = 0; x < length; x++) {
                        if (!board[(x + xStart), yStart].isOccupied) {
                            possiblePlacement[x] = board[(x + xStart), yStart];
                        } else {
                            placed = false;
                            break;
                        }
                    }
                }

                foreach (Tile tile in possiblePlacement) {
                    tile.isOccupied = true;
                    tile.serviceName = serviceName;
                }
                
                Console.WriteLine($"Service {serviceName} has been placed at ({possiblePlacement[0].x}, {possiblePlacement[0].y})");
                services.Add(serviceName, possiblePlacement);
            } else {
                while(!placed) {
                    // clear last placement attempt
                    possiblePlacement = new Tile[length];
                    // assume we can place the service
                    placed = true;
                    // choose a starting point
                    int xStart = rng.Next(boardSize);
                    int yStart = rng.Next(boardSize - length);
                    Console.WriteLine($"Start trying ({xStart}, {yStart})");
                    for (int y = 0; y < length; y++) {
                        if (!board[xStart, (y + yStart)].isOccupied) {
                            possiblePlacement[y] = board[xStart, (y + yStart)];
                        } else {
                            placed = false;
                            break;
                        }
                    }
                }

                foreach (Tile tile in possiblePlacement) {
                    tile.isOccupied = true;
                    tile.serviceName = serviceName;
                }

                Console.WriteLine($"Service {serviceName} has been placed at ({possiblePlacement[0].x}, {possiblePlacement[0].y})");
                services.Add(serviceName, possiblePlacement);
            }
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