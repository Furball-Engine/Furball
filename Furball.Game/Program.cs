using System;
using Console=Furball.Engine.Engine.Console.Console;

namespace Furball.Game {
    public static class Program {
        [STAThread]
        private static void Main() {
            Console.Initialize();
            Console.Run("test_var 123 420 123");

            using (FurballTestGame game = new()) {
                game.Run();
            }
        }
    }
}
