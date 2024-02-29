using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Logging
{
    /// <summary>
    /// Defines sixnet framework log events
    /// </summary>
    public static class SixnetLogEvents
    {
        #region Framework

        public static class Framework
        {
            //system
            public const int SystemError = 51500000;

            //internal queue
            public const int InternalQueueEnqueueError = 51500001;
            public const int InternalQueueConsumeError = 51500002;
            public const int InternalQueueStartConsumer = 51500003;
            public const int InternalQueueCloseConsumer = 51500004;

            //Email
            public const int Email = 51501001;

            //sms
            public const int Sms = 51502001;
        }

        #endregion

        #region Work

        public static class Work
        {
            public const int StartSubmitting = 51510000;
            public const int SubmittedSuccessfully = 51510001;
            public const int SubmittedFailure = 51510002;
            public const int SubmittedException = 51510003;
            public const int ExecutionOptions = 51510004;
            public const int Rollback = 51510005;
            public const int Reset = 51510006;
            public const int Dispose = 51510007;
        }

        #endregion

        #region Activation record

        public static class ActivationRecord
        {
            public const int Inactive = 51520000;
            public const int Break = 51520001;
            public const int GenerateSavingCommand = 51520003;
            public const int GenerateRemovingCommand = 51520004;
            public const int GenerateRemovingByConditionCommand = 51520005;
            public const int GenerateModificationExpressionCommand = 51520006;
        }

        #endregion

        #region Database

        public static class Database
        {
            public const int Script = 51530000;
            public const int NotSetDatabaseServer = 51530001;
            public const int NotModificationValue = 51530002;
        }

        #endregion

        #region Application

        public static class Application
        {
            public const int Initialization = 51540000;
            public const int InitializationFailure = 51540001;
            public const int GetApplicationRootPath = 51540002;
            public const int LoadAssemblyFailure = 51540003;
        }

        #endregion

        #region Cache

        public static class Cache
        {
            public const int NotSetCacheProvider = 51550000;
            public const int CacheOperationResponseMessage = 51550001;
            public const int ConnectionCacheServerError = 51550002;

            //data
            public const int CacheOperationError = 51551001;
            public const int CacheKeyValueIsNullOrEmpty = 51551002;
            public const int NotUsingCache = 51551003;
        }

        #endregion

        #region Configuration

        public static class Configuration
        {
            public const int EntityConfigurationIsNull = 51560000;
            public const int EntityNotSetPrimaryKey = 51560001;
        }

        #endregion
    }
}
