namespace Furball.Engine.Engine.DevConsole.ConFuncs.FurballNative {
    /// <summary>
    /// Not intended for actual Console use, only here so scripts can hook onto this function
    /// </summary>
    public class OnExiting : ConFunc {
        public OnExiting () : base("nt_on_exiting") {}
        public override ConsoleResult Run(string[] consoleInput) => new(ExecutionResult.Success, string.Empty);
    }
}
