using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Sixnet.App;
using Sixnet.DependencyInjection;
using Sixnet.Localization;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Direct throw helper
    /// </summary>
    public static class SixnetDirectThrower
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
        public static void ThrowAppException(bool predicate, string message = "", params string[] args)
        {
            ThrowIf<SixnetApplicationException>(predicate, message, args);
        }

        /// <summary>
        /// Throw application exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowAppException<TResource>(bool predicate, string message = "", params string[] args)
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
            ThrowCoreIf<TException>(predicate, false, message, args);
        }

        /// <summary>
        /// Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="args">Args</param>
        public static void ThrowIf<TResource, TException>(bool predicate, string message = "", params string[] args) where TException : Exception, new()
        {
            ThrowCoreIf<TResource, TException>(predicate, false, message, args);
        }

        /// <summary>
        /// Throw exception core
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="localizationMessage"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void ThrowCoreIf<TException>(bool predicate, bool localizationMessage, string message = "", params string[] args) where TException : Exception, new()
        {
            if (predicate)
            {
                if (!string.IsNullOrWhiteSpace(message)
                    && localizationMessage
                    && SixnetLocalizer.IsLocalizeThrowMessage())
                {
                    message = message.Localize(args);
                }
                var exception = GetException<TException>(message, null);
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Throw exception core
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="localizationMessage"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void ThrowCoreIf<TResource, TException>(bool predicate, bool localizationMessage, string message = "", params string[] args) where TException : Exception, new()
        {
            if (predicate)
            {
                if (!string.IsNullOrWhiteSpace(message)
                    && localizationMessage
                    && SixnetLocalizer.IsLocalizeThrowMessage())
                {
                    message = message.Localize<TResource>(args);
                }
                var exception = GetException<TException>(message, null);
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Get exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <returns></returns>
        static TException GetException<TException>(string message, Exception innerException = null) where TException : Exception
        {
            var exceptionType = typeof(TException);
            var constructorDelegate = _exceptionConstructorCaches.GetOrAdd(exceptionType.GUID.ToString(), _ =>
            {
                var stringParameter = Expression.Parameter(typeof(string));
                var innerExpParameter = Expression.Parameter(typeof(Exception));
                var constructor = exceptionType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(Exception) }, new ParameterModifier[] { });

                var newExceptionExp = Expression.New(constructor, stringParameter, innerExpParameter);
                var lambdaExp = Expression.Lambda<Func<string, Exception, TException>>(newExceptionExp, new ParameterExpression[] { stringParameter, innerExpParameter });
                return lambdaExp.Compile();
            });
            return (TException)constructorDelegate?.Invoke(message, innerException);
        }

        #endregion
    }
}
