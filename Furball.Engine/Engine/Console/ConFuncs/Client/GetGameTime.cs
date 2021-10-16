namespace Furball.Engine.Engine.Console.ConFuncs.Client {
    public class GetGameTime : ConFunc {

        public GetGameTime() : base("cl_get_game_time") {}
        public override (ExecutionResult result, string message) Run(string consoleInput) {
            int currentTime = FurballGame.GameTimeSource.GetCurrentTime();

            return (ExecutionResult.Success, currentTime.ToString());
        }
    }
}
