using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EZNEW.Logging
{
    /// <summary>
    /// Trace log switch manager
    /// </summary>
    public static class TraceLogSwitchManager
    {
        /// <summary>
        /// EZNEW framework switch name
        /// </summary>
        static readonly string EZNEWFrameworkSwitchName = nameof(EZNEWFrameworkSwitchName);

        /// <summary>
        /// Source switchs
        /// </summary>
        static readonly Dictionary<string, SourceSwitch> SourceSwitchs = new Dictionary<string, SourceSwitch>();

        /// <summary>
        /// Add a switch
        /// </summary>
        /// <param name="name">Switch name</param>
        /// <param name="switchValue">Switch value</param>
        public static void AddSwitch(string name, string switchValue)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(switchValue))
            {
                return;
            }
            SourceSwitchs[name] = new SourceSwitch(name, switchValue);
        }

        /// <summary>
        /// Remove switch
        /// </summary>
        /// <param name="name">Switch name</param>
        public static void RemoveSwitch(string name)
        {
            if (SourceSwitchs.ContainsKey(name))
            {
                SourceSwitchs.Remove(name);
            }
        }

        /// <summary>
        /// Enable eznew framework trace log
        /// </summary>
        public static void EnableFrameworkTrace()
        {
            AddSwitch(EZNEWFrameworkSwitchName, "Verbose");
        }

        /// <summary>
        /// Disable eznew framework trace log
        /// </summary>
        public static void DisableFrameworkTrace()
        {
            RemoveSwitch(EZNEWFrameworkSwitchName);
        }

        /// <summary>
        /// Determines whether should output eznew framework trace log
        /// </summary>
        /// <returns>Return whether should output eznew framework trace log</returns>
        public static bool ShouldTraceFramework()
        {
            return ShouldTrace(EZNEWFrameworkSwitchName, TraceEventType.Verbose);
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
            SourceSwitchs.TryGetValue(name, out var sourceSwitch);
            return sourceSwitch?.ShouldTrace(traceEventType) ?? false;
        }
    }
}
