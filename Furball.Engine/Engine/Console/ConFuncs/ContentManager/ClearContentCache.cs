using Furball.Engine.Engine.Graphics;

namespace Furball.Engine.Engine.Console.ConFuncs {
    public class ClearContentCache : ConFunc {
        public ClearContentCache() : base("cmr_clear_cache") {}

        public override string Run(string consoleInput) {
            ContentManager.ClearCache();

            return "Cache cleared!";
        }
    }
}
