using Furball.Vixie.Graphics.Backends;
using Silk.NET.Windowing;

namespace Furball.Game {
    public static class Program {
        private static void Main() {
            WindowOptions options = WindowOptions.Default;
            options.VSync = false;

            using(FurballTestGame game = new FurballTestGame())
                game.Run(options, Backend.OpenGL);
        }
    }
}
