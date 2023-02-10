using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using JetBrains.Annotations;
using Silk.NET.Core.Native;
using Vanara.PInvoke;

namespace Furball.Engine.Engine.Timing;

public class HighResolutionClock : IDisposable {
    public bool Throttled;

    private readonly TimeSpan  _throttledFrameLength;
    private readonly Stopwatch _stopwatch;

    /// <summary>
    /// A waitable timer, only applies to windows
    /// </summary>
    [CanBeNull]
    private Kernel32.SafeWaitableTimerHandle _waitableTimer;

    public HighResolutionClock(TimeSpan throttledFrameLength) {
        this._throttledFrameLength = throttledFrameLength;
        this._stopwatch            = Stopwatch.StartNew();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            this.CreateWaitableTimer();
    }

    private TimeSpan _lastMili = TimeSpan.Zero;

    public void WaitFrame() {
        TimeSpan currentMili = this._stopwatch.Elapsed;

        TimeSpan elapsed = currentMili - this._lastMili;
        TimeSpan toWait  = this._throttledFrameLength - elapsed;
        
        //If the time to have elapsed is less than the time between frames
        if (elapsed < this._throttledFrameLength) {
            bool threadSleep = true;
            if (this._waitableTimer != null) {
                if (Kernel32.SetWaitableTimerEx(this._waitableTimer, CreateFileTime(toWait), 0, null, default, IntPtr.Zero, 0)) {
                    SilkMarshal.WaitWindowsObjects(this._waitableTimer.DangerousGetHandle());
                    threadSleep = false;
                }
            }

            if (threadSleep) {
                Thread.Sleep(toWait);
                TimeSpan waited     = this._stopwatch.Elapsed - currentMili;
                TimeSpan leftToWait = toWait - waited;
                //If we waited less than we should have, spin to wait the rest
                while(leftToWait.Ticks > 0) {
                    waited     = this._stopwatch.Elapsed - currentMili;
                    leftToWait = toWait - waited;
                }
            }
        }

        this._lastMili = this._stopwatch.Elapsed;
    }

    private static FILETIME CreateFileTime(TimeSpan ts) {
        ulong ul = unchecked((ulong)-ts.Ticks);
        return new FILETIME {
            dwHighDateTime = (int)(ul >> 32),
            dwLowDateTime  = (int)(ul & 0xFFFFFFFF)
        };
    }

    private const uint TIMER_ALL_ACCESS = 2031619U;
    private void CreateWaitableTimer() {
        try {
            // Attempt to use a high resolution waitable time, only available since Windows 10, version 1803
            this._waitableTimer = Kernel32.CreateWaitableTimerEx(
            null,
            null,
            Kernel32.CREATE_WAITABLE_TIMER_FLAG.CREATE_WAITABLE_TIMER_MANUAL_RESET | Kernel32.CREATE_WAITABLE_TIMER_FLAG.CREATE_WAITABLE_TIMER_HIGH_RESOLUTION,
            TIMER_ALL_ACCESS
            );

            if (this._waitableTimer == null) {
                // Fall back to a more supported version. This is still far more accurate than Thread.Sleep
                this._waitableTimer = Kernel32.CreateWaitableTimerEx(
                null,
                null,
                Kernel32.CREATE_WAITABLE_TIMER_FLAG.CREATE_WAITABLE_TIMER_MANUAL_RESET,
                TIMER_ALL_ACCESS
                );
            }
        }
        catch {
            //ignored
        }
    }

    public void Dispose() {
        this._waitableTimer?.Dispose();
    }
}
