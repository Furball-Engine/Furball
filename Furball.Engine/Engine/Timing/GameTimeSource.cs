namespace Furball.Engine.Engine.Timing {
    public class GameTimeSource : ITimeSource {

        public int GetCurrentTime() => FurballGame.Time;
    }
}
