using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Sabotage.Views;
using Avalonia.Threading;

namespace Sabotage {
    public class ClientHandle
    {
        // Define a function for each instruction we expect to receive
        public static void Welcome(Packet packet)
        {
            string message = packet.ReadString();
            int myID = packet.ReadInt();

            Console.WriteLine("Message received from server: " + message);
            Client.instance.myID = myID;
            ClientSend.welcomeReceived();
            
            Action connected = delegate() { MainWindow.Connected(); };
            Dispatcher.UIThread.InvokeAsync(connected);
        }

        public static void GameReady(Packet packet)
        {
            GameLogic.myTurn = packet.ReadBool();

            Action updateTurnTracker = delegate() {MainWindow.UpdateTurnTracker();};
            Dispatcher.UIThread.InvokeAsync(updateTurnTracker);

            Console.WriteLine("The game is now ready!");
            if(GameLogic.myTurn) Console.WriteLine("I am going first!");
            else Console.WriteLine("I am going last!");
            
            Action gameReady = delegate() { MainWindow.GameReady(); };
            Dispatcher.UIThread.InvokeAsync(gameReady);
        }

        public static void ReceiveFire(Packet packet) {
            int fromClient = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();

            Console.WriteLine($"Attack from Player {fromClient} at coordinates ({x}, {y})!");
            GameLogic.ReceiveFire(x,y);   
        }

        public static void ConfirmHit(Packet packet) {
            int fromClient = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            bool hitShip = packet.ReadBool();

            if (hitShip) {
                Console.WriteLine($"We hit a service!");
            } else {
                Console.WriteLine($"We missed!");
            }

            GameLogic.TargetHit(x, y, hitShip);
        }

        public static void ConfirmServiceSunk(Packet packet) {
            int fromClient = packet.ReadInt();
            string serviceName = packet.ReadString();

            Console.WriteLine($"We sunk their {serviceName}!");
            Action serviceDown = delegate() { MainWindow.ServiceDown(serviceName); };
            Dispatcher.UIThread.InvokeAsync(serviceDown);
        }

        public static void ConfirmAllServicesSunk(Packet packet) {
            Console.WriteLine("We sunk all their services");
            bool winner = packet.ReadBool();
            Action gameOver = delegate() { MainWindow.GameOver(winner); };
            Dispatcher.UIThread.InvokeAsync(gameOver);
        }
    }
}