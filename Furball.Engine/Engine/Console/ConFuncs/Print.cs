namespace Furball.Engine.Engine.Console.ConFuncs {
    public class Print : ConFunc {
        public Print() : base("print") {

        }

        public override string Run(string consoleInput) {
            return consoleInput;
        }
    }
}
