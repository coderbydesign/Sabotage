using System;

namespace Sabotage {
    public class ClentHandle {
        public static void Welcome(Packet packet) {
            string message = packet.ReadString();
            int myID = packet.ReadInt();

            Console.WriteLine("Message received from server: " + message);
            Client.instance.myID = myID;
            ClientSend.welcomeReceived();
        }
    }
}