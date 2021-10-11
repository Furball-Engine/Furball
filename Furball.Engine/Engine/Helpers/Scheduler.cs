using System.Collections.Generic;
using Furball.Engine.Engine.Helpers.Logger;

namespace Furball.Engine.Engine.Helpers {
    public class Scheduler {
        private readonly List<ScheduledMethod> _scheduledMethods = new();

        public Scheduler() {
            Logger.Logger.Log("Scheduler Initialized!", new LoggerLevelSchedulerInfo());
        }

        public void Update(int time) {
            for (int i = 0; i < this._scheduledMethods.Count; i++) {
                ScheduledMethod method = this._scheduledMethods[i];
                if (time > method.Time) {
                    Logger.Logger.Log($"ScheduledMethod invoked at {time}", new LoggerLevelSchedulerInfo());
                    method.MethodToRun.Invoke(time);
                    this._scheduledMethods.Remove(method);
                }
            }
        }

        public void ScheduleMethod(ScheduledMethod.Method method, int time, bool runOnDispose = false) {
            Logger.Logger.Log($"ScheduledMethod scheduled at {time}, {(runOnDispose ? "does" : "does not")} run on dispose", new LoggerLevelSchedulerInfo());

            this._scheduledMethods.Add(new(method, time, runOnDispose));
        }

        public void Dispose(int time) {
            Logger.Logger.Log($"Scheduler disposed at {time}", new LoggerLevelSchedulerInfo());

            foreach (ScheduledMethod method in this._scheduledMethods) {
                Logger.Logger.Log($"ScheduledMethod invoked at {time} during dispose", new LoggerLevelSchedulerInfo());
                method.MethodToRun.Invoke(time);
            }

            this._scheduledMethods.Clear();
        }
    }

    public class ScheduledMethod {
        public delegate void Method(int time);

        public readonly Method MethodToRun;
        public readonly bool   RunOnDispose;
        public readonly int    Time;

        protected internal ScheduledMethod(Method method, int time, bool runOnDispose = false) {
            this.MethodToRun  = method;
            this.Time         = time;
            this.RunOnDispose = runOnDispose;
        }
    }
}
