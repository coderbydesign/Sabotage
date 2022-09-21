using System;

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
    }
}