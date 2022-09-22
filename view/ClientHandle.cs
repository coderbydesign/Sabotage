using System;
using Sabotage.Views;

namespace Sabotage {
    public class ClentHandle {
        // Define a function for each instruction we expect to receive
        public static void Welcome(Packet packet) {
            string message = packet.ReadString();
            int myID = packet.ReadInt();

            Console.WriteLine("Message received from server: " + message);
            Client.instance.myID = myID;
            ClientSend.welcomeReceived();
        }

        public static void ServerFull(Packet packet) {
            Console.WriteLine("The server is now full");
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