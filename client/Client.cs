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
            tcp.Connect();
        }

        public class TCP {
            public TcpClient socket;
            private NetworkStream stream;
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
                socket.EndConnect(result);

                if(!socket.Connected) {
                    return;
                }

                stream = socket.GetStream();
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult _result) {
                try {
                    int byteLength = stream.EndRead(_result);
                    if(byteLength <= 0) {
                        return;
                    }
                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                } catch (Exception e) {
                    Console.WriteLine("Error receiving TCP");
                }
            }
        }
    }
}