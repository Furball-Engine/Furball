using System.Collections.Generic;
using Furball.Engine.Engine.Helpers.Logger;

namespace Furball.Engine.Engine.Helpers; 

public class Scheduler {
    private readonly List<ScheduledMethod> _scheduledMethods = new();

    public Scheduler() {
        Kettu.Logger.Log("Scheduler Initialized!", LoggerLevelSchedulerInfo.Instance);
    }

    public void Update(double time) {
        for (int i = 0; i < this._scheduledMethods.Count; i++) {
            ScheduledMethod method = this._scheduledMethods[i];
            if (time > method.Time) {
                Kettu.Logger.Log($"ScheduledMethod invoked at {time}", LoggerLevelSchedulerInfo.Instance);
                method.MethodToRun.Invoke(time);
                this._scheduledMethods.Remove(method);
            }
        }
    }

    public void ScheduleMethod(ScheduledMethod.Method method, double time, bool runOnDispose = false) {
        Kettu.Logger.Log($"ScheduledMethod scheduled at {time}, {(runOnDispose ? "does" : "does not")} run on dispose", LoggerLevelSchedulerInfo.Instance);

        this._scheduledMethods.Add(new ScheduledMethod(method, time, runOnDispose));
    }

    public void Dispose(double time) {
        Kettu.Logger.Log($"Scheduler disposed at {time}", LoggerLevelSchedulerInfo.Instance);

        foreach (ScheduledMethod method in this._scheduledMethods) {
            Kettu.Logger.Log($"ScheduledMethod invoked at {time} during dispose", LoggerLevelSchedulerInfo.Instance);
            method.MethodToRun.Invoke(time);
        }

        this._scheduledMethods.Clear();
    }
}

public class ScheduledMethod {
    public delegate void Method(double time);

    public readonly Method MethodToRun;
    public readonly bool   RunOnDispose;
    public readonly double Time;

    protected internal ScheduledMethod(Method method, double time, bool runOnDispose = false) {
        this.MethodToRun  = method;
        this.Time         = time;
        this.RunOnDispose = runOnDispose;
    }
}