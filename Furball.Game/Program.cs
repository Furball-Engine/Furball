using System;
using System.Threading;
using Furball.Engine;
using Console=Furball.Engine.Engine.Console.Console;

namespace Furball.Game {
    public static class Program {
        private static void Main() {
            FurballTestGame[] games = new FurballTestGame[4];

            for (int i = 0; i != 4; i++) {
                new Thread(
                () => {
                    games[i] = new FurballTestGame();
                    games[i].Run();
                }).Start();
            }

            while (true) {
                Thread.Sleep(1500);
            }
        }
    }
}
