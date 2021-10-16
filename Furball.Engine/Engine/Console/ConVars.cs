using System;
using Furball.Engine.Engine.Console.ConFuncs;
using Furball.Engine.Engine.Console.ConFuncs.Standard;
using Furball.Engine.Engine.Console.Types;
using Furball.Engine.Engine.Platform;

namespace Furball.Engine.Engine.Console {
    public class ConVars {
        public static IntIntConVar ScreenResolution = new("cl_screen_resolution", $"{FurballGame.DEFAULT_WINDOW_WIDTH} {FurballGame.DEFAULT_WINDOW_HEIGHT}");
        public static IntConVar    DebugOverlay     = new("cl_debug_overlay", RuntimeInfo.IsDebug() ? 1 : 0);
        public static IntConVar    TargetFps        = new("cl_target_fps", -1, OnTargetFpsChange);
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

        #region ConVar OnChanges
        //TODO: move to a function hook once those work
        private static void OnTargetFpsChange() {
            int value = TargetFps.Value.Value;

            if(value != -1)
                FurballGame.Instance.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / (double) value);
            else
                FurballGame.Instance.TargetElapsedTime = TimeSpan.FromTicks(1);
        }

        #endregion
    }
}
