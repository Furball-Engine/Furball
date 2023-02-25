namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class InputUpdateRate : DebugCounterItem {
    private double _passedTime;
    private int    _inputFramesPerSecond;

    public override void Update(double time) {
        base.Update(time);

        this._passedTime += time;

        if (this._passedTime > 1000d) {
            this._passedTime -= 1000d;

            this._inputFramesPerSecond = FurballGame.InputManager.CountedInputFrames;

            FurballGame.InputManager.CountedInputFrames = 0;
        }
    }
    
    public override string GetAsString(double time) => $"{this._inputFramesPerSecond:N0}ifps ({1000.0 / this._inputFramesPerSecond:N2}ms)";
}
