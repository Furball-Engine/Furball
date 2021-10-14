using Furball.Engine.Engine.Console.ConFuncs;
using Furball.Engine.Engine.Console.Types;

namespace Furball.Engine.Engine.Console {
    public class ConVars {
        public static StringConVar TestVar          = new("test_var");
        public static IntIntConVar ScreenResolution = new("screen_resolution");

        public static ConFunc QuitFunction      = new Quit();
        public static ConFunc ScreenResFunction = new SetScreenResolution();
    }
}
