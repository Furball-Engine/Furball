using Furball.Engine.Engine.Console.Types;

namespace Furball.Engine.Engine.Console {
    public class ConVars {
        public static StringConVar TestVar         = new("test_var");
        public static IntConVar ResolutionWidth = new("screen_resolution_width");
        public static IntConVar ResolutionHeight = new("screen_resolution_height");
    }
}
