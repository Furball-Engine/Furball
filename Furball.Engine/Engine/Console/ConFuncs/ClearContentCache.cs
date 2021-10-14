using Furball.Engine.Engine.Graphics;

namespace Furball.Engine.Engine.Console.ConFuncs {
    public class ClearContentCache : ConFunc {
        public ClearContentCache() : base("clear_content_cache") {}

        public override string Run(string consoleInput) {
            ContentManager.ClearCache();

            return "Cache cleared!";
        }
    }
}
