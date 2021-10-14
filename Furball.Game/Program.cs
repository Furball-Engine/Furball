using System;

namespace Furball.Game {
    public static class Program {
        [STAThread]
        private static void Main() {
            using (FurballTestGame game = new()) {
                game.Run();
            }
        }
    }
}
