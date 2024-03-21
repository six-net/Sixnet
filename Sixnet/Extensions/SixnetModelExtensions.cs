﻿using Sixnet.Constants;
using Sixnet.Development.Work;
using Sixnet.Exceptions;
using System.Collections.Generic;

namespace Sixnet.Model
{
    public static class SixnetModelExtensions
    {
        #region Extra parameter

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="parameterModel">Parameter option</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="overCurrent">Indicates whether over current value when current parameter value is not null or empty</param>
        public static void AddParameter(this ISixnetProperties parameterModel, string name, string value, bool overCurrent = false)
        {
            if (parameterModel == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            string nowParameterValue = string.Empty;
            parameterModel.Properties?.TryGetValue(name, out nowParameterValue);
            if (!string.IsNullOrWhiteSpace(nowParameterValue) && !overCurrent)
            {
                return;
            }
            if (parameterModel.Properties == null)
            {
                parameterModel.Properties = new Dictionary<string, string>();
            }
            parameterModel.Properties[name] = value;
        }

        /// <summary>
        /// Add work id
        /// It will use the WorkManager.Current.WorkId when 'workId' is null or empty
        /// </summary>
        /// <param name="parameterModel">Parameter model</param>
        /// <param name="workId">Work id</param>
        /// <param name="overCurrent">Indicates whether over current value when current parameter value is not null or empty</param>
        public static void AddWorkId(this ISixnetProperties parameterModel, string workId = "", bool overCurrent = false)
        {
            //work id
            string currentWorkId = workId;
            if (string.IsNullOrWhiteSpace(currentWorkId))
            {
                currentWorkId = UnitOfWork.Current?.WorkId;
            }
            AddParameter(parameterModel, SixnetExtraParameterNames.WorkId, currentWorkId, overCurrent);
        }

        /// <summary>
        /// Add template message id
        /// </summary>
        /// <param name="parameterModel">Parameter model</param>
        /// <param name="messageId">Message id</param>
        /// <param name="overCurrent">Whether over current value when current parameter value is not null or empty</param>
        public static void AddTemplateMessageId(this ISixnetProperties parameterModel, string messageId, bool overCurrent = false)
        {
            AddParameter(parameterModel, SixnetExtraParameterNames.TemplateMessageId, messageId, overCurrent);
        }

        #endregion
    }
}