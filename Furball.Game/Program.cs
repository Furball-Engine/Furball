using Furball.Vixie;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Vixie.Backends.Veldrid;
using Silk.NET.Windowing;

namespace Furball.Game {
    public static class Program {
        private static void Main() {
            WindowOptions options = WindowOptions.Default;
            options.VSync = false;

            GraphicsBackend.PrefferedBackends = Backend.Veldrid;

            VeldridBackend.PrefferedBackend = Veldrid.GraphicsBackend.OpenGL;
            
            using(FurballTestGame game = new FurballTestGame())
                game.Run(options);
        }
    }
}
