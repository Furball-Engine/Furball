using System;
using System.Diagnostics;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

internal class MemoryUsage : DebugCounterItem {
    private long _managedMemory = GC.GetTotalMemory(false);
    private long _unmanagedMemory;

    private readonly Process _process;

    public MemoryUsage() {
        this._process = Process.GetCurrentProcess();
    }

    public override void Dispose() {
        base.Dispose();

        this._process.Dispose();
    }

    public override string GetAsString(double time) {
        this._process.Refresh();

        this._managedMemory   = GC.GetTotalMemory(true);
        this._unmanagedMemory = this._process.PrivateMemorySize64 - this._managedMemory;

        return $"mmu: {this._managedMemory / 1024f:N2}kb umu: {this._unmanagedMemory / 1024f:N2}kb";
    }
}
