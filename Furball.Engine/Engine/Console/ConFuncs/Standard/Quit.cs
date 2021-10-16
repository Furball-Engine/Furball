namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class Quit: ConFunc {
        public Quit() : base("quit") {

        }

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            FurballGame.Instance.Exit();
            return (ExecutionResult.Success, "Exiting game.");
        }
    }
}
