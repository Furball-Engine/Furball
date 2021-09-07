using System;

namespace Foxfire.Game {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new FoxfireTestGame())
                game.Run();
        }
    }
}
