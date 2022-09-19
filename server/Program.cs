using System;

namespace Sabotage {
    class Program {
        static void Main(string[] args) {
            Console.Title = "Sabotage!";
            Server.Start(2, 25565);
            Console.ReadKey();
        }
    }
}