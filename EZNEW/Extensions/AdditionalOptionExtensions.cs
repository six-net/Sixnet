using EZNEW.Develop.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Diagnostics
{
    public static class AdditionalOptionExtensions
    {
        /// <summary>
        /// Add additional value
        /// </summary>
        /// <param name="additionalOption">Additional option</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="overCurrent">Whether over current value when current parameter value is not null or empty</param>
        public static void AddAdditionalValue(this IAdditionalOption additionalOption, string name, string value, bool overCurrent = false)
        {
            if (additionalOption == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            string nowParameterValue = string.Empty;
            additionalOption.Additionals?.TryGetValue(name, out nowParameterValue);
            if (!string.IsNullOrWhiteSpace(nowParameterValue) && !overCurrent)
            {
                return;
            }
            if (additionalOption.Additionals == null)
            {
                additionalOption.Additionals = new Dictionary<string, string>();
            }
            additionalOption.Additionals[name] = value;
        }

        /// <summary>
        /// Add work id
        /// It will use the WorkManager.Current.WorkId when 'workId' is null or empty
        /// </summary>
        /// <param name="additionalOption">Additional option</param>
        /// <param name="workId">Work id</param>
        /// <param name="overCurrent">Whether over current value when current parameter value is not null or empty</param>
        public static void AddWorkId(this IAdditionalOption additionalOption, string workId = "", bool overCurrent = false)
        {
            //work id
            string currentWorkId = workId;
            if (string.IsNullOrWhiteSpace(currentWorkId))
            {
                currentWorkId = WorkManager.Current?.WorkId;
            }
            AddAdditionalValue(additionalOption, EZNEWConstants.AdditionalParameterNames.WorkId, currentWorkId, overCurrent);
        }

        /// <summary>
        /// Add template message id
        /// </summary>
        /// <param name="additionalOption">Additional option</param>
        /// <param name="messageId">Message id</param>
        /// <param name="overCurrent">Whether over current value when current parameter value is not null or empty</param>
        public static void AddTemplateMessageId(this IAdditionalOption additionalOption, string messageId, bool overCurrent = false)
        {
            AddAdditionalValue(additionalOption, EZNEWConstants.AdditionalParameterNames.TemplateMessageId, messageId, overCurrent);
        }
    }
}
