using System;

namespace Sabotage {
    class Program {
        static void Main(string[] args) {
            Console.Title = "Sabotage!";
            Client client = new Client();
            client.ConnectToServer();
            Console.ReadKey();
        }
    }
}