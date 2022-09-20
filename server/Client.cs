using System;
using System.Net;
using System.Net.Sockets;

namespace Sabotage {
    class Client {
        public static int dataBufferSize = 4096;
        public int playerID;
        public TCP tcp;

        public Client(int clientID) {
            playerID = clientID;
            tcp = new TCP(playerID);
        }

        public class TCP {
            public TcpClient socket;
            private int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet receivedData;

            public TCP(int _id) {
                id = _id;
            }

            public void Connect(TcpClient _socket) {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();
                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                ServerSend.Welcome(id, "Welome to the Server");
            }

            public void SendData(Packet packet) {
                try {
                    if(socket != null) {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Error sending packet to player " + id + " via TCP: " + e);
                }
            }

            // This function is responsible for reading data sent to client
            private void ReceiveCallback(IAsyncResult _result) {
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
                            Server.packetHandlers[packetID](id, packet);
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
    }
}