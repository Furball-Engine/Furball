using Furball.Engine.Engine.Console.ConFuncs.Client;
using Furball.Engine.Engine.Console.ConFuncs.ContentManager;
using Furball.Engine.Engine.Console.ConFuncs.Standard;
using Furball.Engine.Engine.Console.Types;
using Furball.Engine.Engine.Platform;

namespace Furball.Engine.Engine.Console {
    public class ConVars {
        /// <summary>
        /// Client Screen Resolution
        /// </summary>
        public static IntIntConVar ScreenResolution = new("cl_screen_resolution", $"{FurballGame.DEFAULT_WINDOW_WIDTH} {FurballGame.DEFAULT_WINDOW_HEIGHT}");
        /// <summary>
        /// Whether or not the Debug Overlay should be displayed
        /// </summary>
        public static IntConVar    DebugOverlay     = new("cl_debug_overlay", RuntimeInfo.IsDebug() ? 1 : 0);
        /// <summary>
        /// Target FPS for the Client
        /// </summary>
        public static IntConVar    TargetFps        = new("cl_target_fps", -1);
        /// <summary>
        /// Whether or not the Console should be logged and saved upon closing the game
        /// </summary>
        public static IntConVar    WriteLog         = new("cl_console_log", 1);
    }
}
