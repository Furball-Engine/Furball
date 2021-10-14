namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class Quit: ConFunc {
        public Quit() : base("quit") {

        }

        public override string Run(string consoleInput) {
            FurballGame.Instance.Exit();
            return "Exiting game!";
        }
    }
}
