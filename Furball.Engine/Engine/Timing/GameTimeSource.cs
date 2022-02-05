namespace Furball.Engine.Engine.Timing {
    /// <summary>
    /// A Time Source which gets the Raw Time of the Game
    /// </summary>
    public class GameTimeSource : ITimeSource {
        public double GetCurrentTime() => FurballGame.Time;
    }
}
