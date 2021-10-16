namespace Furball.Engine.Engine.DevConsole.ConFuncs.FurballNative {
    /// <summary>
    /// Not intended for actual Console use, only here so scripts can hook onto this function
    /// </summary>
    public class OnExiting : ConFunc {
        public OnExiting () : base("nt_on_exiting") {}
        public override (ExecutionResult result, string message) Run(string consoleInput) => (ExecutionResult.Success, string.Empty);
    }
}
