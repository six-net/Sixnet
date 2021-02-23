using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW
{
    /// <summary>
    /// Defines constants
    /// </summary>
    public static class EZNEWConstants
    {
        /// <summary>
        /// Defines switch names
        /// </summary>
        public static class SwitchNames
        {
            /// <summary>
            /// EZNEW framework trace log switch name
            /// </summary>
            public const string FrameworkTraceLog = nameof(FrameworkTraceLog);
        }

        /// <summary>
        /// Defines additional parameter names
        /// </summary>
        public static class AdditionalParameterNames
        {
            /// <summary>
            /// UnitOfWork id
            /// </summary>
            public const string WorkId = nameof(WorkId);

            /// <summary>
            /// Template message id
            /// </summary>
            public const string TemplateMessageId = nameof(TemplateMessageId);
        }

        /// <summary>
        /// Defines message receiver types
        /// </summary>
        public static class MessageReceiverTypes
        {
            public const string Email = nameof(Email);

            public const string Mobile = nameof(Mobile);

            public const string Object = nameof(Object);

            public const string Other = nameof(Other);
        }

        /// <summary>
        /// Defines internal queue names
        /// </summary>
        public static class InternalQueueNames
        {
            public const string DataCache = "EZNEW_DATA_CACHE";

            public const string Logging = "EZNEW_LOGGING";

            public const string Message = "EZNEW_MESSAGE";
        }
    }
}
