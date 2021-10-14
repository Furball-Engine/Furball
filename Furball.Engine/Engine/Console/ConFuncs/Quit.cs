namespace Furball.Engine.Engine.Console.ConFuncs {
    public class Quit: ConFunc {
        public Quit() : base("quit") {

        }

        public override string Run(string consoleInput) {
            FurballGame.Instance.Exit();
            return "Exiting game!";
        }
    }
}
