using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MSC_Remote_Control_Mod
{
    public class ResetInstance
    {
        public DateTime EndTime { get; }

        private Action Action { get; }

        public ResetInstance(DateTime endTime, Action action)
        {
            this.EndTime = endTime;
            this.Action = action;
        }

        public void Execute()
        {
            Action.Invoke();
        }
    }
    
    public static class ResetHandler
    {
        private static readonly List<ResetInstance> ResetInstances = new List<ResetInstance>();
        // ReSharper disable once UnusedMember.Local
        private static Timer _timer = new Timer(CheckAndResetValues, ResetInstances, 0, 1000);
        
        private static void CheckAndResetValues(object state)
        {
            var instances = (List<ResetInstance>)state;
            var currentTime = DateTime.Now;

            var toRemove = new List<ResetInstance>();
            foreach (var instance in instances.Where(instance => currentTime >= instance.EndTime))
            {
                instance.Execute();
                toRemove.Add(instance);
            }

            instances.RemoveAll(instance => toRemove.Contains(instance));
        }

        public static void ResetAfter(DateTime after, Action action)
        {
            ResetInstances.Add(new ResetInstance(after, action));
        }
    }
}