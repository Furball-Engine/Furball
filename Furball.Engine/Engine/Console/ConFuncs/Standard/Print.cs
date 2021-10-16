namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class Print : ConFunc {
        public Print() : base("print") {

        }

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            return (ExecutionResult.Success, consoleInput);
        }
    }
}
