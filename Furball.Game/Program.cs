using System;

namespace Furball.Game {
    public static class Program {
        private static void Main() {
            using(FurballTestGame game = new FurballTestGame())
                game.Run();
        }
    }
}
