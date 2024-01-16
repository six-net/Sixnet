using System.Collections.Generic;
using Sixnet.Constants;
using Sixnet.Development.Work;
using Sixnet.Model;

namespace Sixnet.Configuration
{
    public static class ParameterOptionExtensions
    {
        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="parameterOptions">Parameter option</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="overCurrent">Indicates whether over current value when current parameter value is not null or empty</param>
        public static void AddParameter(this IParameterOptions parameterOptions, string name, string value, bool overCurrent = false)
        {
            if (parameterOptions == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            string nowParameterValue = string.Empty;
            parameterOptions.Parameters?.TryGetValue(name, out nowParameterValue);
            if (!string.IsNullOrWhiteSpace(nowParameterValue) && !overCurrent)
            {
                return;
            }
            if (parameterOptions.Parameters == null)
            {
                parameterOptions.Parameters = new Dictionary<string, string>();
            }
            parameterOptions.Parameters[name] = value;
        }

        /// <summary>
        /// Add work id
        /// It will use the WorkManager.Current.WorkId when 'workId' is null or empty
        /// </summary>
        /// <param name="parameterOptions">Parameter option</param>
        /// <param name="workId">Work id</param>
        /// <param name="overCurrent">Indicates whether over current value when current parameter value is not null or empty</param>
        public static void AddWorkId(this IParameterOptions parameterOptions, string workId = "", bool overCurrent = false)
        {
            //work id
            string currentWorkId = workId;
            if (string.IsNullOrWhiteSpace(currentWorkId))
            {
                currentWorkId = UnitOfWork.Current?.WorkId;
            }
            AddParameter(parameterOptions, InternalParameterNames.WorkId, currentWorkId, overCurrent);
        }

        /// <summary>
        /// Add template message id
        /// </summary>
        /// <param name="parameterOptions">Parameter option</param>
        /// <param name="messageId">Message id</param>
        /// <param name="overCurrent">Whether over current value when current parameter value is not null or empty</param>
        public static void AddTemplateMessageId(this IParameterOptions parameterOptions, string messageId, bool overCurrent = false)
        {
            AddParameter(parameterOptions, InternalParameterNames.TemplateMessageId, messageId, overCurrent);
        }
    }
}
