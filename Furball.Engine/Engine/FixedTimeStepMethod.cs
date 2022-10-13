using System;

namespace Furball.Engine.Engine; 

public class FixedTimeStepMethod {
    /// <summary>
    /// The miliseconds per call
    /// </summary>
    public double Tempo;

    public delegate void MethodDefinition();

    /// <summary>
    /// The method to call every `Tempo` miliseconds
    /// </summary>
    public MethodDefinition Method;
        
    private double _delta;

    public FixedTimeStepMethod(double tempo, MethodDefinition method) {
        this.Tempo  = tempo;
        this.Method = method;
    }

    internal void Update(double delta) {
        this._delta += delta;

        int timesToRun = (int)Math.Floor(this._delta / this.Tempo);

        for (int i = 0; i < timesToRun; i++) {
            this.Method.Invoke();
            this._delta -= this.Tempo;
        }
    }
}