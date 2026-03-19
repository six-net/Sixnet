// "Company © 2025. All rights reserved."

using System.Collections.Concurrent;

using Sixnet.Localization;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Throw helper
    /// </summary>
    public static partial class SixnetThrower
    {
        #region Sixnet exception

        /// <summary>
        /// Throw sixnet exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowSixnetExceptionIf(bool predicate, string message = null, params string[] args)
        {
            ThrowIf<SixnetException>(predicate, message, args);
        }

        /// <summary>
        /// Throw application exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowAppExceptionIf(bool predicate, string message = null, params string[] args)
        {
            ThrowIf<SixnetApplicationException>(predicate, message, args);
        }

        /// <summary>
        /// Throw application exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        /// <param name="args">Args</param>
        public static void ThrowAppExceptionIf<TResource>(bool predicate, string message = null, params string[] args)
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
        public static void ThrowArgNullIf(bool predicate, string message = null)
        {
            ThrowIf<ArgumentNullException>(predicate, message);
        }

        /// <summary>
        /// Throw an argument null exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowArgNullIf<TResource>(bool predicate, string message = null)
        {
            ThrowIf<TResource, ArgumentNullException>(predicate, message);
        }

        /// <summary>
        /// Throw an argument exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        public static void ThrowArgErrorIf(bool predicate, string message = null)
        {
            ThrowIf<ArgumentException>(predicate, message);
        }

        /// <summary>
        /// Throw an argument exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Exception message</param>
        public static void ThrowArgErrorIf<TResource>(bool predicate, string message = null)
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
        public static void ThrowInvalidOperationIf(bool predicate, string message = null)
        {
            ThrowIf<InvalidOperationException>(predicate, message);
        }

        /// <summary>
        /// Throw an invalid operation exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowInvalidOperationIf<TResource>(bool predicate, string message = null)
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
        public static void ThrowNotSupportIf(bool predicate, string message = null)
        {
            ThrowIf<NotSupportedException>(predicate, message);
        }

        /// <summary>
        /// Throw a not support exception
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="message">Message</param>
        public static void ThrowNotSupportIf<TResource>(bool predicate, string message = null)
        {
            ThrowIf<TResource, NotSupportedException>(predicate, message);
        }

        #endregion

        #region Save failed

        /// <summary>
        /// Throw save failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowSaveFailedIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.save_data_failed;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Add failed

        /// <summary>
        /// Throw add failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowAddFailedIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.add_data_failed;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Get failed

        /// <summary>
        /// Throw get failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowGetDataFailedIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.get_data_failed;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Update failed

        /// <summary>
        /// Throw update failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowUpdateFailedIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.update_data_failed;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Delete failed

        /// <summary>
        /// Throw delete failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowDeleteFailedIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.delete_data_failed;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Unspecified data

        /// <summary>
        /// Throw unspecified data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowUnspecifiedIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.unspecified_data;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Has data

        /// <summary>
        /// Throw exist data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        public static void ThrowExistIf<T>(bool predicate, params string[] args)
        {
            var message = SixnetResourceKeys.exist_value;
            if (args.IsNullOrEmpty())
            {
                args = [SixnetLocalizer.GetString(typeof(T).Name.ToResourceKey())];
            }
            ThrowAppExceptionIf(predicate, message, args);
        }

        #endregion

        #region Generic

        /// <summary>
        /// Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="args">Args</param>
        public static void ThrowIf<TException>(bool predicate, string message = null, params string[] args) where TException : Exception, new()
        {
            SixnetDirectThrower.ThrowCoreIf<TException>(predicate, true, message, args);
        }

        /// <summary>
        /// Throw exception
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="args">Args</param>
        public static void ThrowIf<TResource, TException>(bool predicate, string message = null, params string[] args) where TException : Exception, new()
        {
            SixnetDirectThrower.ThrowCoreIf<TResource, TException>(predicate, true, message, args);
        }

        #endregion
    }
}
