namespace Furball.Engine.Engine.DevConsole.ConFuncs.ContentManager {
    public class ClearContentCache : ConFunc {
        /// <summary>
        /// cmr_clear_cache
        /// Clears the ContentManager Content Cache
        /// </summary>
        public ClearContentCache() : base("cmr_clear_cache") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            Graphics.ContentManager.ClearCache();

            return (ExecutionResult.Success, "ContentManager cache has been cleared.");
        }
    }
}
