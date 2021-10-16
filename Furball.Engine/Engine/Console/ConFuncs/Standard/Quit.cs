namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    /// <summary>
    /// `quit`
    /// Exits the Game
    /// </summary>
    public class Quit: ConFunc {
        public Quit() : base("quit") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            FurballGame.Instance.Exit();

            return (ExecutionResult.Success, "Exiting game.");
        }
    }
}
