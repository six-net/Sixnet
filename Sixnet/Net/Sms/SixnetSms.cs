using System;
using System.Collections.Generic;
using System.Linq;
using Sixnet.DependencyInjection;
using Sixnet.Development.Message;
using Sixnet.Exceptions;
using Sixnet.Model;
using Sixnet.Net.Email;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms manager
    /// </summary>
    public static partial class SixnetSms
    {
        #region Fields

        /// <summary>
        /// Gets the default tag
        /// </summary>
        public const string DefaultTag = "SIXNET_SMS_DEFAULT_TAG";

        /// <summary>
        /// Sms options
        /// </summary>
        readonly static SmsOptions _defaultSmsOptions = new();

        #endregion

        #region Send sms

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="parameters">Send sms parameters</param>
        /// <returns></returns>
        public static List<SendSmsResult> Send(IEnumerable<SendSmsParameter> parameters)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameters.IsNullOrEmpty(), nameof(parameters));

            var smsOptions = GetSmsOptions();
            SmsAccount smsAccount = null;
            var results = new List<SendSmsResult>();
            var smsProvider = GetSmsProvider();

            foreach (var parameter in parameters)
            {
                if (parameter == null)
                {
                    continue;
                }
                if (!smsOptions.UseSameAccount || smsAccount == null)
                {
                    smsAccount = GetSmsAccount(smsOptions, parameter);
                    if (smsAccount == null)
                    {
                        continue;
                    }
                }
                var sendResult = Execute(smsAccount, parameter, smsProvider.Send);
                results.Add(sendResult);
            }
            smsOptions.SendCallback?.Invoke(results);
            return results;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public static SendSmsResult Send(SendSmsParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            return Send(new SendSmsParameter[1] { parameter })?.FirstOrDefault();
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="tag">Sms tag</param>
        /// <param name="content">Content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Receive mobiles</param>
        /// <returns></returns>
        public static SendSmsResult Send(string subject, string tag, string content
            , object parameters, params string[] mobiles)
        {
            return Send(new SendSmsParameter()
            {
                Mobiles = mobiles,
                Subject = subject,
                Tag = tag,
                Content = content,
                Properties = parameters.ToStringDictionary()
            });
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="tag">Sms tag</param>
        /// <param name="content">Sms content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobile numbers</param>
        /// <returns></returns>
        public static SendSmsResult Send(string tag, string content, object parameters, params string[] mobiles)
        {
            return Send(string.Empty, tag, content, parameters, mobiles);
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="content">Sms content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobile numbers</param>
        /// <returns></returns>
        public static SendSmsResult Send(string content, object parameters, params string[] mobiles)
        {
            return Send(string.Empty, DefaultTag, content, parameters, mobiles);
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Send sms parameter</param>
        /// <returns></returns>
        public static SendSmsResult Send(SmsAccount account, SendSmsParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));
            SixnetDirectThrower.ThrowArgNullIf(parameter != null, nameof(parameter));

            var smsProvider = GetSmsProvider();
            return Execute(account, parameter, smsProvider.Send);
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="content">Content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobiles</param>
        /// <returns></returns>
        public static SendSmsResult Send(SmsAccount account, string content, object parameters, params string[] mobiles)
        {
            return Send(account, new SendSmsParameter()
            {
                Content = content,
                Properties = parameters?.ToStringDictionary(),
                Tag = DefaultTag,
                Mobiles = mobiles
            });
        }

        #endregion

        #region Receive sms

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="parameter">Receive sms parameter</param>
        /// <returns>Return receive sms result</returns>
        public static ReceiveSmsResult Receive(ReceiveSmsParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return Execute(smsAccount, parameter, smsProvider.Receive);
        }

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Receive sms parameter</param>
        /// <returns>Return receive sms result</returns>
        public static ReceiveSmsResult Receive(SmsAccount account, ReceiveSmsParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return Execute(account, parameter, smsProvider.Receive);
        }

        #endregion

        #region Get sms report

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="parameter">Get sms report parameter</param>
        /// <returns>Return sms report result</returns>
        public static GetSmsReportResult GetSmsReport(GetSmsReportParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return Execute(smsAccount, parameter, smsProvider.GetSmsReport);
        }

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Get sms report parameter</param>
        /// <returns>Return sms report result</returns>
        public static GetSmsReportResult GetSmsReport(SmsAccount account, GetSmsReportParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return Execute(account, parameter, smsProvider.GetSmsReport);
        }

        #endregion

        #region Query balance

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="parameter">Query balance parameter</param>
        /// <returns>Return query balance result</returns>
        public static QuerySmsBalanceResult QueryBalance(QueryBalanceParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return Execute(smsAccount, parameter, smsProvider.QueryBalance);
        }

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Query balance parameter</param>
        /// <returns>Return query balance result</returns>
        public static QuerySmsBalanceResult QueryBalance(SmsAccount account, QueryBalanceParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return Execute(account, parameter, smsProvider.QueryBalance);
        }

        #endregion

        #region Check black

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="parameter">Check black parameter</param>
        /// <returns>Return check black result</returns>
        public static CheckBlackResult CheckBlack(CheckBlackParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return Execute(smsAccount, parameter, smsProvider.CheckBlack);
        }

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="parameter">Check black parameter</param>
        /// <returns>Return check black result</returns>
        public static CheckBlackResult CheckBlack(SmsAccount account, CheckBlackParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return Execute(account, parameter, smsProvider.CheckBlack);
        }

        #endregion

        #region Check keyword

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="parameter">Check keyword parameter</param>
        /// <returns>Return check keyword result</returns>
        public static CheckKeywordResult CheckKeyword(CheckKeywordParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return Execute(smsAccount, parameter, smsProvider.CheckKeyword);
        }

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        public static CheckKeywordResult CheckKeyword(SmsAccount account, CheckKeywordParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return Execute(account, parameter, smsProvider.CheckKeyword);
        }

        #endregion

        #region Send template message

        /// <summary>
        /// Send template message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SendSmsResult SendTemplateMessage(SendMessageContext context)
        {
            return Send(GetSmsParameter(context.Template, context.Message, context.Receivers));
        }

        #endregion

        #region Util

        static SendSmsParameter GetSmsParameter(MessageTemplate template, MessageInfo message, IEnumerable<string> mobiles)
        {
            SixnetDirectThrower.ThrowArgNullIf(message == null, nameof(message));
            SixnetDirectThrower.ThrowArgNullIf(mobiles.IsNullOrEmpty(), nameof(mobiles));

            // Parameters
            var parameterDict = message.Data?.ToStringDictionary();
            var templateParameters = SixnetMessager.GetTemplateParameters(parameterDict);

            // Content
            var contentResolveResult = SixnetMessager.ResolveTemplate(template.Content, templateParameters);
            if (!contentResolveResult.Success || string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName))
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName)
                    , $"Not set '{contentResolveResult.ErrorParameterName}' value in the sms template");
            }

            // Sms parameter
            var smsParameter = new SendSmsParameter()
            {
                Subject = message.Subject,
                Content = contentResolveResult.NewContent,
                Mobiles = mobiles,
                Properties = parameterDict
            };
            smsParameter.AddWorkId(message.WorkId);
            smsParameter.AddTemplateMessageId(message.Id);
            return smsParameter;
        }

        /// <summary>
        /// Execute sms operation
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="smsAccount"></param>
        /// <param name="smsParameter"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        internal static TResult Execute<TParameter, TResult>(SmsAccount smsAccount, TParameter smsParameter
            , Func<SmsAccount, TParameter, TResult> proxy) where TParameter : SmsParameter where TResult : SmsResult
        {
            var result = proxy(smsAccount, smsParameter);
            if (result != null)
            {
                result.SmsAccount = smsAccount;
                result.SmsOptions = smsParameter;
            }
            return result;
        }

        /// <summary>
        /// Get sms provider
        /// </summary>
        /// <returns></returns>
        static ISixnetSmsProvider GetSmsProvider()
        {
            var smsProvider = SixnetContainer.GetService<ISixnetSmsProvider>();
            SixnetDirectThrower.ThrowSixnetExceptionIf(smsProvider == null, "Not set sms provider");
            return smsProvider;
        }

        /// <summary>
        /// Get sms account
        /// </summary>
        /// <param name="smsParameter">Sms parameter</param>
        /// <returns></returns>
        static SmsAccount GetSmsAccount(SmsOptions smsOptions, SmsParameter smsParameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(smsParameter == null, nameof(smsParameter));

            var smsAccount = smsOptions?.GetSmsAccount(smsParameter) ?? smsOptions.Account;

            SixnetDirectThrower.ThrowSixnetExceptionIf(smsAccount == null, "No set sms account");

            return smsAccount;
        }

        /// <summary>
        /// Get sms options
        /// </summary>
        /// <returns></returns>
        static SmsOptions GetSmsOptions()
        {
            return SixnetContainer.GetOptions<SmsOptions>() ?? _defaultSmsOptions;
        }

        #endregion
    }
}
