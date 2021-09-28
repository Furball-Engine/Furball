namespace Furball.Engine.Engine.Timing {
    public class LoopingTimeSource : ITimeSource {
        public ITimeSource BaseTimeSource;
        public int         LoopTime;
        
        public LoopingTimeSource(ITimeSource baseTimeSource, int loopTime) {
            this.BaseTimeSource = baseTimeSource;
            this.LoopTime       = loopTime;
        }
        
        public int GetCurrentTime() => this.BaseTimeSource.GetCurrentTime() % this.LoopTime;
    }
}
