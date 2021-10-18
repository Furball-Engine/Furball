namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
    /// <summary>
    /// cl_get_game_time
    /// Gets the FurballGame.GameTimeSource time
    /// </summary>
    public class GetGameTime : ConFunc {
        public GetGameTime() : base("cl_get_game_time") {}

        public override ConsoleResult Run(string[] consoleInput) {
            int currentTime = FurballGame.GameTimeSource.GetCurrentTime();

            return new ConsoleResult(ExecutionResult.Success, currentTime.ToString());
        }
    }
}
