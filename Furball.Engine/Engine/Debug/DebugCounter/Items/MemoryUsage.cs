using System;
using System.Diagnostics;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

public class MemoryUsage : DebugCounterItem {
    private readonly FixedTimeStepMethod _updateTimeStep;

    private long _managedMemory = GC.GetTotalMemory(false);
    private long _unmanagedMemory;

    private readonly Process _process;

    public MemoryUsage() {
        this._process = Process.GetCurrentProcess();

        FurballGame.TimeStepMethods.Add(
        this._updateTimeStep = new FixedTimeStepMethod(
        1000,
        () => {
            this._process.Refresh();
            
            this._managedMemory   = GC.GetTotalMemory(true);
            this._unmanagedMemory = this._process.PrivateMemorySize64 - this._managedMemory;
        }
        )
        );
    }

    public override void Dispose() {
        base.Dispose();

        this._process.Dispose();
        
        FurballGame.TimeStepMethods.Remove(this._updateTimeStep);
    }

    public override string GetAsString(double time) => $"mmu: {this._managedMemory / 1024f:N2}kb umu: {this._unmanagedMemory / 1024f:N2}kb";
}
