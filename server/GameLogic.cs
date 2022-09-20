using System;

// The purpose of this class is to be ready to receive packets
namespace Sabotage {
    public class GameLogic {
        public static void Update() {
            ThreadManager.UpdateMain();
        }
    }
}