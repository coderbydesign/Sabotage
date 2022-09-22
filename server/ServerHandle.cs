using System;

namespace Sabotage {
    public class ServerHandle {
        // Define a method for each type of instruction we expect
        public static void WelcomeReceived(int fromClient, Packet packet) {
            int clientID = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine(Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint + " connected and is now player " + fromClient);
            if (fromClient != clientID) {
                Console.WriteLine("Player " + username + " (ID: " + fromClient + ") has used the wrong client ID: " + clientID);
            }
        }

        public static void FireReceived(int fromClient, Packet packet) {
            int clientID = packet.ReadInt();
            int x = packet.ReadInt();
            int y = packet.ReadInt();

            Console.WriteLine($"player {clientID} fired at ({x}, {y})");

            // Logic for finding who we are passing the signal along too
            int otherPlayer;
            if (clientID == 1) otherPlayer = 2;
            else otherPlayer = 1;

            ServerSend.Fire(otherPlayer, x, y);
        }
    }
}