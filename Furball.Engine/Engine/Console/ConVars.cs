using System;
using Furball.Engine.Engine.Console.ConFuncs;
using Furball.Engine.Engine.Console.ConFuncs.Client;
using Furball.Engine.Engine.Console.ConFuncs.ContentManager;
using Furball.Engine.Engine.Console.ConFuncs.Standard;
using Furball.Engine.Engine.Console.Types;
using Furball.Engine.Engine.Platform;

namespace Furball.Engine.Engine.Console {
    public class ConVars {
        public static IntIntConVar ScreenResolution = new("cl_screen_resolution", $"{FurballGame.DEFAULT_WINDOW_WIDTH} {FurballGame.DEFAULT_WINDOW_HEIGHT}");
        public static IntConVar    DebugOverlay     = new("cl_debug_overlay", RuntimeInfo.IsDebug() ? 1 : 0);
        public static IntConVar    TargetFps        = new("cl_target_fps", -1);
        public static IntConVar    WriteLog         = new("cl_console_log", 1);
        /// <summary>
        /// `quit`
        /// Exits the Game
        /// </summary>
        public static ConFunc QuitFunction      = new Quit();
        /// <summary>
        /// `cl_set_screen_resolution`
        /// Changes the Screen Resolution
        /// Syntax: `cl_set_screen_resolution width height`
        /// </summary>
        public static ConFunc ScreenResFunction = new SetScreenResolution();
        /// <summary>
        /// `print`
        /// Prints a line to console
        /// </summary>
        public static ConFunc PrintFunction     = new Print();
        /// <summary>
        /// cmr_clear_cache
        /// Clears the ContentManager Content Cache
        /// </summary>
        public static ConFunc ClearContentCache = new ClearContentCache();
        /// <summary>
        /// create_var
        /// Creates a Variable
        /// Syntax: `create_var variable_type variable_name`
        /// </summary>
        public static ConFunc CreateVariable = new CreateVar();
        /// <summary>
        /// delete_var
        /// Deletes a Variable
        /// Syntax: `delete_var variable_name`
        /// <remarks>Only Variables that have been created with `create_var` are allowed to be deleted</remarks>
        /// </summary>
        public static ConFunc DeleteVariable = new DeleteVar();
        /// <summary>
        /// hook
        /// Establishes a Hook (works in a similiar way to Subscribing to OnChange on a Bindable) on a Function/Variable, upon Calling/Changing the Hooked Variable/Function the Hook Gets Invoked
        /// Syntax: `hook +(variable/function) hook_target hook_action`
        /// </summary>
        public static ConFunc Hook           = new Hook();
        /// <summary>
        /// cl_set_target_fps
        /// Sets the Target Frame Rate
        /// Syntax: `cl_set_target_fps target_frame_rate`
        /// <remarks>if called without parameters, it will set the frame rate to whatever `cl_target_fps` is</remarks>
        /// <remarks>if called with the value of -1, it sets it to unlimited</remarks>
        /// </summary>
        public static ConFunc SetTargetFps = new SetTargetFps();
    }
}
