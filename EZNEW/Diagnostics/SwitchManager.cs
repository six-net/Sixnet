using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EZNEW.Diagnostics
{
    /// <summary>
    /// Switch manager
    /// </summary>
    public static class SwitchManager
    {

        /// <summary>
        /// Source switchs
        /// </summary>
        static readonly Dictionary<string, Switch> Switchs = new Dictionary<string, Switch>();

        /// <summary>
        /// Source switch listeners
        /// </summary>
        static readonly Dictionary<string, Action<Switch>> SwitchListeners = new Dictionary<string, Action<Switch>>();

        /// <summary>
        /// Add a switch
        /// </summary>
        /// <param name="switchName">Switch name</param>
        /// <param name="switchValue">Switch value</param>
        public static void AddSwitch(string switchName, string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchName) || string.IsNullOrWhiteSpace(switchValue))
            {
                return;
            }
            var switchObj = new SourceSwitch(switchName, switchValue);
            Switchs[switchName] = switchObj;
            TriggerSwitchListener(switchName);
        }

        /// <summary>
        /// Remove switch
        /// </summary>
        /// <param name="switchName">Switch name</param>
        public static void RemoveSwitch(string switchName)
        {
            if (Switchs.ContainsKey(switchName))
            {
                Switchs.Remove(switchName);
                TriggerSwitchListener(switchName);
                RemoveSwitchListener(switchName);
            }
        }

        /// <summary>
        /// Enable eznew framework trace log
        /// </summary>
        public static void EnableFrameworkTrace()
        {
            AddSwitch(EZNEWConstants.SwitchNames.FrameworkTraceLog, nameof(TraceEventType.Verbose));
        }

        /// <summary>
        /// Disable eznew framework trace log
        /// </summary>
        public static void DisableFrameworkTrace()
        {
            RemoveSwitch(EZNEWConstants.SwitchNames.FrameworkTraceLog);
        }

        /// <summary>
        /// Determines whether should output the EZNEW.NET framework trace log
        /// </summary>
        /// <param name="listenAction">Switch listen action</param>
        /// <returns>Return whether should output eznew framework trace log</returns>
        public static bool ShouldTraceFramework(Action<Switch> listenAction = null)
        {
            if (listenAction != null)
            {
                AddSwitchListener(EZNEWConstants.SwitchNames.FrameworkTraceLog, listenAction);
            }
            return ShouldTrace(EZNEWConstants.SwitchNames.FrameworkTraceLog, TraceEventType.Verbose);
        }

        /// <summary>
        /// Determines whether should output trace log
        /// </summary>
        /// <param name="name">Switch name</param>
        /// <param name="traceEventType">Trace event type</param>
        public static bool ShouldTrace(string name, TraceEventType traceEventType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            Switchs.TryGetValue(name, out var sourceSwitch);
            return (sourceSwitch as SourceSwitch)?.ShouldTrace(traceEventType) ?? false;
        }

        /// <summary>
        /// Add switch listener
        /// </summary>
        /// <param name="switchName">Switch name</param>
        /// <param name="action">Action</param>
        public static void AddSwitchListener(string switchName, Action<Switch> action)
        {
            if (action == null || string.IsNullOrWhiteSpace(switchName))
            {
                return;
            }
            if (SwitchListeners.ContainsKey(switchName))
            {
                SwitchListeners[switchName] += action;
            }
            else
            {
                SwitchListeners[switchName] = action;
            }
        }

        /// <summary>
        /// Remove swtich listener
        /// </summary>
        /// <param name="switchName">Switch name</param>
        public static void RemoveSwitchListener(string switchName)
        {
            if (string.IsNullOrWhiteSpace(switchName))
            {
                return;
            }
            if (SwitchListeners.ContainsKey(switchName))
            {
                SwitchListeners.Remove(switchName);
            }
        }

        /// <summary>
        /// Clear switch listener
        /// </summary>
        public static void ClearSwitchListener()
        {
            SwitchListeners?.Clear();
        }

        /// <summary>
        /// Trigger switch listener
        /// </summary>
        /// <param name="switchName">Switch name</param>
        static void TriggerSwitchListener(string switchName)
        {
            SwitchListeners.TryGetValue(switchName, out var listener);
            Switchs.TryGetValue(switchName, out var switchObj);
            listener?.Invoke(switchObj);
        }
    }
}
