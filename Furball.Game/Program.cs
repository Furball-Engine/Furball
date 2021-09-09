using System;

namespace Furball.Game {
    public static class Program {
        [STAThread]
        static void Main() {
            using (FurballTestGame game = new FurballTestGame())
                game.Run();
        }
    }
}
