using Sixnet.App;
using Sixnet.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Throw helper
    /// </summary>
    public static class SixnetThrower
    {
        #region Fields

        /// <summary>
        /// Exception constructor caches
        /// </summary>
        readonly static ConcurrentDictionary<string, Func<string, Exception, Exception>> _exceptionConstructorCaches = new();

        #endregion

        #region Sixnet exception

        /// <summary>
        /// Throw sixnet exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowSixnetExceptionIf(bool predicate, string message = "", params string[] args)
        {
            ThrowIf<SixnetException>(predicate, message, args);
        }

        /// <summary>
        /// Throw application exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowAppExceptionIf(bool predicate, string message = "", params string[] args)
        {
            ThrowIf<SixnetApplicationException>(predicate, message, args);
        }

        /// <summary>
        /// Throw application exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowAppExceptionIf<TResource>(bool predicate, string message = "", params string[] args)
        {
            ThrowIf<TResource, SixnetApplicationException>(predicate, message, args);
        }

        #endregion

        #region Argument exception

        /// <summary>
        /// Throw an argument null exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowArgNullIf(bool predicate, string message = "")
        {
            ThrowIf<ArgumentNullException>(predicate, message);
        }

        /// <summary>
        /// Throw an argument null exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowArgNullIf<TResource>(bool predicate, string message = "")
        {
            ThrowIf<TResource, ArgumentNullException>(predicate, message);
        }

        /// <summary>
        /// Throw an argument exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        public static void ThrowArgErrorIf(bool predicate, string message = "")
        {
            ThrowIf<ArgumentException>(predicate, message);
        }

        /// <summary>
        /// Throw an argument exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        public static void ThrowArgErrorIf<TResource>(bool predicate, string message = "")
        {
            ThrowIf<TResource, ArgumentException>(predicate, message);
        }

        #endregion

        #region Operation exception

        /// <summary>
        /// Throw an invalid operation exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowInvalidOperationIf(bool predicate, string message = "")
        {
            ThrowIf<InvalidOperationException>(predicate, message);
        }

        /// <summary>
        /// Throw an invalid operation exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowInvalidOperationIf<TResource>(bool predicate, string message = "")
        {
            ThrowIf<TResource, InvalidOperationException>(predicate, message);
        }

        #endregion

        #region Not support exception

        /// <summary>
        /// Throw a not support exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowNotSupportIf(bool predicate, string message = "")
        {
            ThrowIf<NotSupportedException>(predicate, message);
        }

        /// <summary>
        /// Throw a not support exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowNotSupportIf<TResource>(bool predicate, string message = "")
        {
            ThrowIf<TResource, NotSupportedException>(predicate, message);
        }

        #endregion

        #region Generic

        /// <summary>
        /// Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="args">Args</param>
        public static void ThrowIf<TException>(bool predicate, string message = "", params string[] args) where TException : Exception, new()
        {
            SixnetDirectThrower.ThrowCoreIf<TException>(predicate, true, message, args);
        }

        /// <summary>
        /// Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="args">Args</param>
        public static void ThrowIf<TResource, TException>(bool predicate, string message = "", params string[] args) where TException : Exception, new()
        {
            SixnetDirectThrower.ThrowCoreIf<TResource, TException>(predicate, true, message, args);
        }

        #endregion
    }
}
