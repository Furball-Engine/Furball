using Silk.NET.Windowing;

namespace Furball.Game {
    public static class Program {
        private static void Main() {
            WindowOptions options = WindowOptions.Default;
            options.VSync = false;
            options.API   = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 3));

            using(FurballTestGame game = new FurballTestGame())
                game.Run(options);
        }
    }
}
