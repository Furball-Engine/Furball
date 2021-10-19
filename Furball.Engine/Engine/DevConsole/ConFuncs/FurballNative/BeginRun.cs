namespace Furball.Engine.Engine.DevConsole.ConFuncs.FurballNative {
    /// <summary>
    /// Not intended for actual Console use, only here so scripts can hook onto this function
    /// </summary>
    public class BeginRun : ConFunc {
        public BeginRun() : base("nt_begin_run") {}
        public override ConsoleResult Run(string[] consoleInput) => new(ExecutionResult.Success, string.Empty);
    }
}
