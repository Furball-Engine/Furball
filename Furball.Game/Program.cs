using System;

namespace Furball.Game {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new FurballTestGame())
                game.Run();
        }
    }
}
