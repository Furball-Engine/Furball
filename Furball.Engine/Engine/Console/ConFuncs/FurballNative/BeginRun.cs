namespace Furball.Engine.Engine.Console.ConFuncs.FurballNative {
    /// <summary>
    /// Not intended for actual Console use, only here so scripts can hook onto this function
    /// </summary>
    public class BeginRun : ConFunc {
        public BeginRun() : base("nt_begin_run") {}
        public override (ExecutionResult result, string message) Run(string consoleInput) => (ExecutionResult.Success, string.Empty);
    }
}
