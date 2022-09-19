using System;
using System.Net;
using System.Net.Sockets;

namespace Sabotage {
    class Client {
        public static int dataBufferSize = 4096;
        public int playerID;
        public TCP tcp;

        public class TCP {
            public TcpClient socket;
            private int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;

            public TCP(int _id) {
                id = _id;
            }

            public void Connect(TcpClient _socket) {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receiveBuffer = new byte[dataBufferSize];

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
