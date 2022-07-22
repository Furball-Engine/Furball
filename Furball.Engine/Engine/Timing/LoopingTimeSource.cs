namespace Furball.Engine.Engine.Timing; 

public class LoopingTimeSource : ITimeSource {
    public ITimeSource BaseTimeSource;
    public double      LoopTime;
        
    public LoopingTimeSource(ITimeSource baseTimeSource, double loopTime) {
        this.BaseTimeSource = baseTimeSource;
        this.LoopTime       = loopTime;
    }
        
    public double GetCurrentTime() => this.BaseTimeSource.GetCurrentTime() % this.LoopTime;
}