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
            if (method == null) {
                //TODO: dont break here, figure out how to iterate after removing everything
                this._scheduledMethods.RemoveAll(x => x == null);
                break;
            }
            if (method.Cancel) {
                //TODO: figure out how to not have to break while removing
                this._scheduledMethods.Remove(method);
                break;
            }
            if (time > method.Time) {
                Kettu.Logger.Log($"ScheduledMethod invoked at {time}", LoggerLevelSchedulerInfo.Instance);
                this._scheduledMethods.Remove(method);
                method.MethodToRun.Invoke(time);
            }
        }
    }

    public ScheduledMethod ScheduleMethod(ScheduledMethod.Method method, double time = 0, bool runOnDispose = false) {
        Kettu.Logger.Log($"ScheduledMethod scheduled at {time}, {(runOnDispose ? "does" : "does not")} run on dispose", LoggerLevelSchedulerInfo.Instance);

        ScheduledMethod scheduledMethod = new(method, time, runOnDispose);
        this._scheduledMethods.Add(scheduledMethod);
        return scheduledMethod;
    }

    public void Dispose(double time) {
        Kettu.Logger.Log($"Scheduler disposed at {time}", LoggerLevelSchedulerInfo.Instance);

        for (int i = 0; i < this._scheduledMethods.Count; i++) {
            ScheduledMethod method = this._scheduledMethods[i];

            this._scheduledMethods.RemoveAt(i);

            i--;
            if (i < 0) i = 0;
            
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

    public bool Cancel = false;

    protected internal ScheduledMethod(Method method, double time, bool runOnDispose = false) {
        this.MethodToRun  = method;
        this.Time         = time;
        this.RunOnDispose = runOnDispose;
    }
}