using Furball.Engine.Engine.Graphics;

namespace Furball.Engine.Engine.Console.ConFuncs {
    public class ClearContentCache : ConFunc {
        public ClearContentCache() : base("cmr_clear_cache") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            ContentManager.ClearCache();

            return (ExecutionResult.Success, "ContentManager cache has been cleared.");
        }
    }
}
