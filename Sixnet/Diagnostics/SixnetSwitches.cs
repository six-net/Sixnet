using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sixnet.Constants;

namespace Sixnet.Diagnostics
{
    /// <summary>
    /// Switches
    /// </summary>
    public static class SixnetSwitches
    {
        /// <summary>
        /// Source switchs
        /// </summary>
        static readonly Dictionary<string, Switch> _switchs = new Dictionary<string, Switch>();

        /// <summary>
        /// Source switch listeners
        /// </summary>
        static readonly Dictionary<string, Action<Switch>> _switchListeners = new Dictionary<string, Action<Switch>>();

        /// <summary>
        /// Add a switch
        /// </summary>
        /// <param name="switchName">Switch name</param>
        /// <param name="switchValue">Switch value</param>
        public static void Add(string switchName, string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchName) || string.IsNullOrWhiteSpace(switchValue))
            {
                return;
            }
            var switchObj = new SourceSwitch(switchName, switchValue);
            _switchs[switchName] = switchObj;
            TriggerSwitchListener(switchName);
        }

        /// <summary>
        /// Remove switch
        /// </summary>
        /// <param name="switchName">Switch name</param>
        public static void Remove(string switchName)
        {
            if (_switchs.ContainsKey(switchName))
            {
                _switchs.Remove(switchName);
                TriggerSwitchListener(switchName);
                Unlisten(switchName);
            }
        }

        /// <summary>
        /// Trace framework log
        /// </summary>
        public static void TraceFramework()
        {
            Add(SwitchNames.SixnetTraceLog, nameof(TraceEventType.Verbose));
        }

        /// <summary>
        /// Untrace framework log
        /// </summary>
        public static void UntraceFramework()
        {
            Remove(SwitchNames.SixnetTraceLog);
        }

        /// <summary>
        /// Determines whether should output the framework trace log
        /// </summary>
        /// <param name="listenAction">Switch listen action</param>
        /// <returns>Return whether should output framework trace log</returns>
        public static bool ShouldTraceFramework(Action<Switch> listenAction = null)
        {
            if (listenAction != null)
            {
                Listen(SwitchNames.SixnetTraceLog, listenAction);
            }
            return ShouldTrace(SwitchNames.SixnetTraceLog, TraceEventType.Verbose);
        }

        /// <summary>
        /// Determines whether should output trace log
        /// </summary>
        /// <param name="switchName">Switch name</param>
        /// <param name="traceEventType">Trace event type</param>
        public static bool ShouldTrace(string switchName, TraceEventType traceEventType)
        {
            if (string.IsNullOrWhiteSpace(switchName))
            {
                return false;
            }
            _switchs.TryGetValue(switchName, out var sourceSwitch);
            return (sourceSwitch as SourceSwitch)?.ShouldTrace(traceEventType) ?? false;
        }

        /// <summary>
        /// Listen switch
        /// </summary>
        /// <param name="switchName">Switch name</param>
        /// <param name="action">Action</param>
        public static void Listen(string switchName, Action<Switch> action)
        {
            if (action == null || string.IsNullOrWhiteSpace(switchName))
            {
                return;
            }
            if (_switchListeners.ContainsKey(switchName))
            {
                _switchListeners[switchName] += action;
            }
            else
            {
                _switchListeners[switchName] = action;
            }
        }

        /// <summary>
        /// Unlisten switch
        /// </summary>
        /// <param name="switchName">Switch name</param>
        public static void Unlisten(string switchName)
        {
            if (string.IsNullOrWhiteSpace(switchName))
            {
                return;
            }
            if (_switchListeners.ContainsKey(switchName))
            {
                _switchListeners.Remove(switchName);
            }
        }

        /// <summary>
        /// Unlisten all switch
        /// </summary>
        public static void UnlistenAll()
        {
            _switchListeners?.Clear();
        }

        /// <summary>
        /// Trigger switch listener
        /// </summary>
        /// <param name="switchName">Switch name</param>
        static void TriggerSwitchListener(string switchName)
        {
            _switchListeners.TryGetValue(switchName, out var listener);
            _switchs.TryGetValue(switchName, out var switchObj);
            listener?.Invoke(switchObj);
        }

        /// <summary>
        /// Switch names
        /// </summary>
        public static class SwitchNames
        {
            /// <summary>
            /// Sixnet framework trace log switch name
            /// </summary>
            public const string SixnetTraceLog = nameof(SixnetTraceLog);
        }
    }
}
