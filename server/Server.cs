using System;
using System.Net;
using System.Net.Sockets;

namespace Sabotage {
    class Server {
        public static int MaxPlayers;
        private static TcpListener socket;
        private static int port = 25565;

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static void Start(int _MaxPlayers, int _port) {
            MaxPlayers = _MaxPlayers;
            port = _port;

            Console.WriteLine("Starting server on port " + port + "...");

            InitializeServerData();

            // Prepare and open the server to accept clients
            socket = new TcpListener(IPAddress.Any, port);
            socket.Start();
            socket.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null);
        }

        private static void ClientConnected(IAsyncResult result) {
            TcpClient client = socket.EndAcceptTcpClient(result);
            client.NoDelay = false;
            Console.WriteLine("Incoming connection from " + client.Client.RemoteEndPoint);

            socket.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null);
            // Look for an empty slot for a player
            for (int i = 1; i <= MaxPlayers; i++) {
                if (clients[i].tcp.socket  == null) {
                    clients[i].tcp.Connect(client);
                    if (i == MaxPlayers) {
                        ServerSend.GameReady();
                    }
                    return;
                }
            }

            Console.WriteLine("SERVER IS FULL");
        }

        // This function instantiates "slots" in the server for players to join
        private static void InitializeServerData() {
            for (int i = 1; i <= MaxPlayers; i++) {
                clients.Add(i, new Client(i));
            }

            // Add new instruction pairs for each type of packet we expect to receive
            packetHandlers = new Dictionary<int, PacketHandler>() {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                {(int)ClientPackets.fire, ServerHandle.FireReceived},
                {(int)ClientPackets.confirmHit, ServerHandle.ConfirmHit}
            };

            Console.WriteLine("Initialized Packets.");
        }
    }
}