// Responsible for defining packets to be sent over the network
using System;

namespace Sabotage {
    class ServerSend {
        // This prepares the packet to be sent
        private static void SendTCPData(int toClient, Packet packet) {
            // inserts the length of the packet to the front of it so the receivers knows how to read it
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        // Send packet to every connected player
        private static void SendTCPDataToAll(Packet packet) {
            // inserts the length of the packet to the front of it so the receivers knows how to read it
            packet.WriteLength();

            for(int i = 1; i <= Server.MaxPlayers; i++) {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        // Send packet to every connected player EXCEPT the one listed
        private static void SendTCPDataToAll(int exceptClient, Packet packet) {
            // inserts the length of the packet to the front of it so the receivers knows how to read it
            packet.WriteLength();

            for(int i = 1; i <= Server.MaxPlayers; i++) {
                if(i != exceptClient) {
                    Server.clients[i].tcp.SendData(packet);
                }
            }
        }

        // Define a new function for each type of instruction we expect to send
        public static void Welcome(int toClient, string message) {
            // Packet class is disposable, this disposes it automatically
            // welcome is the packet ID
            using (Packet packet = new Packet((int)ServerPackets.welcome)) {
                packet.Write(message);
                packet.Write(toClient);
                SendTCPData(toClient, packet);
            }
        }


        public static void Fire(int toClient, int x, int y) {
            using (Packet packet = new Packet((int)ServerPackets.fire)) {
                packet.Write(toClient);
                packet.Write(x);
                packet.Write(y);

                SendTCPData(toClient, packet);
            }
        }

        public static void GameReady(int startingPlayer) {
            using (Packet packet = new Packet((int)ServerPackets.gameReady)) {
                packet.Write(true);
                SendTCPData(startingPlayer, packet);
            }

            using (Packet packet = new Packet((int)ServerPackets.gameReady)) {
                packet.Write(false);
                int otherPlayer;
                if (startingPlayer == 1) otherPlayer = 2;
                else otherPlayer = 1;
                SendTCPData(otherPlayer, packet);
            }
        }

        public static void ConfirmHit(int toClient, int x, int y, bool hitShip) {
            using (Packet packet = new Packet((int)ServerPackets.confirmHit)) {
                packet.Write(toClient);
                packet.Write(x);
                packet.Write(y);
                packet.Write(hitShip);

                SendTCPData(toClient, packet);
            }
        }

        public static void ConfirmServiceSunk(int toClient, string serviceName) {
            using (Packet packet = new Packet((int)ServerPackets.serviceSunk)) {
                packet.Write(toClient);
                packet.Write(serviceName);
                SendTCPData(toClient, packet);
            }
        }

        public static void ConfirmAllServicesSunk(int toClient) {
            using (Packet packet = new Packet((int)ServerPackets.allServicesSunk)) {
                SendTCPData(toClient, packet);
            }
        }
    }
}