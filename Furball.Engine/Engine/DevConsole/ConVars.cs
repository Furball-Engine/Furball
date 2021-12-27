using Furball.Engine.Engine.Platform;

namespace Furball.Engine.Engine.DevConsole {
    public class ConVars : ConVarStore {
        public static bool DebugOverlay = RuntimeInfo.IsDebug();
        
        /// <summary>
        /// Whether or not the Console should be logged and saved upon closing the game
        /// </summary>
        public static bool WriteLog = true;
        /// <summary>
        ///     Whether or not to enable tool tips when hovering over things
        /// </summary>
        /// 
        public static bool ToolTips = true;
    }
}
