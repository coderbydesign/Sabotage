using System;
using System.Net;
using System.Net.Sockets;

namespace Sabotage {
    public class Client {
        public static Client instance;
        public static int dataBufferSize = 4096;
        public string ip = "127.0.0.1";
        public int port = 25565;
        public int myID = 1;
        public TCP tcp;
        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public Client() {
            if (instance == null) {
                instance = this;
                instance.Start();
            } 
        }

        private void Start() {
            tcp = new TCP();
        }

        public void ConnectToServer() {
            InitializeClientData();
            tcp.Connect();
        }

        public class TCP {
            public TcpClient socket;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect() {
                socket = new TcpClient {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult result) {
                Console.WriteLine("Connected callback");
                socket.EndConnect(result);

                if(!socket.Connected) {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Packet packet) {
                try {
                    if(socket != null) {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Error sending data: " + e);
                }
            }

            // This function is responsible for reading data sent to client
            private void ReceiveCallback(IAsyncResult _result) {
                Console.WriteLine("Received data from TCP");
                try {
                    int byteLength = stream.EndRead(_result);
                    if(byteLength <= 0) {
                        return;
                    }
                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    // Keep reading until we run out of data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                } catch (Exception e) {
                    Console.WriteLine("Error receiving TCP: " + e.ToString());
                }
            }

            private bool HandleData(byte[] data) {
                int packetLength = 0;
                receivedData.SetBytes(data);
                
                // We are checking if at least an int (size 4 bytes) was sent
                // because that's the first part of the packet we send, the length of the packet
                if(receivedData.UnreadLength() >= 4) {
                    packetLength = receivedData.ReadInt();

                    // if we have an empty packet, tell the received data it can now reset
                    if(packetLength <= 0) {
                        return true;
                    }
                }

                // loop through the packet until we reach the end
                while(packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);

                        using (Packet packet = new Packet(packetBytes)) {
                            int packetID = packet.ReadInt();
                            packetHandlers[packetID](packet);
                        }

                    packetLength = 0;

                    if(receivedData.UnreadLength() >= 4) {
                        packetLength = receivedData.ReadInt();

                        // if we have an empty packet, tell the received data it can now reset
                        if(packetLength <= 0) {
                            return true;
                        }
                    }
                }

                if (packetLength <= 1) {
                    return true;
                }

                return false;
            }
        }

        private void InitializeClientData() {
            packetHandlers = new Dictionary<int, PacketHandler>() {
                {(int)ServerPackets.welcome, ClentHandle.Welcome}
            };

            Console.WriteLine("Initialized packets");
        }
    }
}