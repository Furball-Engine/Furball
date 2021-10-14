namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class Print : ConFunc {
        public Print() : base("print") {

        }

        public override string Run(string consoleInput) {
            return consoleInput;
        }
    }
}
