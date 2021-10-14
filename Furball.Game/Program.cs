using System;
using Furball.Engine.Engine.Console;
using Console=Furball.Engine.Engine.Console.Console;

namespace Furball.Game {
    public static class Program {
        [STAThread]
        private static void Main() {
            Console.Initialize();

            using (FurballTestGame game = new()) {
                game.Run();
            }
        }
    }
}
