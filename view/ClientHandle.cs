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
            Console.WriteLine("The game is now ready!");
        }

        public static void ReceiveFire(Packet packet) {
            int fromClient = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();

            Console.WriteLine($"Attack from Player {fromClient} add coordinates ({x}, {y})!");
            GameLogic.ReceiveFire(x,y);   
        }
    }
}