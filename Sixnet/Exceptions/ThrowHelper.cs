using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Sixnet.Cache.Server;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Throw helper
    /// </summary>
    public static class ThrowHelper
    {
        #region Fields

        readonly static ConcurrentDictionary<string, Func<string, Exception, Exception>> ExceptionConstructorCaches = new();

        #endregion

        #region Framework exception

        /// <summary>
        /// Throw eznew framework exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowFrameworkErrorIf(bool predicate, string message = "")
        {
            SixnetException.ThrowIf(predicate, message);
        }

        #endregion

        #region Argument exception

        /// <summary>
        /// Throw an argument null exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        public static void ThrowArgNullIf(bool predicate, string message = "")
        {
            if (predicate)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Throw an argument exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        public static void ThrowArgErrorIf(bool predicate, string message = "")
        {
            if (predicate)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Throw value is null or empty
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowNullOrEmptyIf(bool predicate, string message = "")
        {
            ThrowArgErrorIf(predicate, $"{message} is null or empty");
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
            if (predicate)
            {
                throw new InvalidOperationException(message);
            }
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
            if (predicate)
            {
                throw new NotSupportedException(message);
            }
        }

        #endregion

        #region Generic

        /// <summary>
        /// Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowIf<TException>(bool predicate, string message = "") where TException : Exception, new()
        {
            if (predicate)
            {
                var exception = GetException<TException>(message, null);
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        static TException GetException<TException>(string message, Exception innerException = null) where TException : Exception
        {
            var exceptionType = typeof(TException);
            var constructorDelegate = ExceptionConstructorCaches.GetOrAdd(exceptionType.GUID.ToString(), _ =>
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
