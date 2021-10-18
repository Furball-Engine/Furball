using Furball.Engine.Engine.DevConsole.Types;
using Furball.Engine.Engine.Platform;

namespace Furball.Engine.Engine.DevConsole {
    public class ConVars : ConVarStore {
        /// <summary>
        /// Client Screen Resolution
        /// </summary>
        public static IntIntConVar ScreenResolution        = new("cl_screen_resolution", $"{FurballGame.DEFAULT_WINDOW_WIDTH}:{FurballGame.DEFAULT_WINDOW_HEIGHT}");
        /// <summary>
        /// Whether or not the Debug Overlay should be displayed
        /// </summary>
        public static IntConVar    DebugOverlay            = new("cl_debug_overlay", RuntimeInfo.IsDebug() ? 1 : 0);
        /// <summary>
        /// Target FPS for the Client
        /// </summary>
        public static IntConVar    TargetFps               = new("cl_target_fps", 1000);
        /// <summary>
        /// How much to scale down the Frame Rate when the Window is Inactive
        /// </summary>
        public static DoubleConVar TargetFpsUnfocusedScale = new("cl_fps_unfocused_scale", 0.25);
        /// <summary>
        /// Whether or not the Console should be logged and saved upon closing the game
        /// </summary>
        public static IntConVar    WriteLog                = new("cl_console_log", 1);
    }
}
