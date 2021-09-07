using System;

namespace Foxfire.Engine {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new FoxfireGame())
                game.Run();
        }
    }
}
