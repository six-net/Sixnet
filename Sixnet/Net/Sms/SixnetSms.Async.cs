using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Exceptions;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms manager
    /// </summary>
    public static partial class SixnetSms
    {
        #region Send sms

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="parameters">Send sms parameters</param>
        /// <returns></returns>
        public static async Task<List<SendSmsResult>> SendAsync(IEnumerable<SendSmsParameter> parameters)
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
                var sendResult = await ExecuteAsync(smsAccount, parameter, smsProvider.SendAsync).ConfigureAwait(false);
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
        public static async Task<SendSmsResult> SendAsync(SendSmsParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            return (await SendAsync(new SendSmsParameter[1] { parameter }).ConfigureAwait(false))?.FirstOrDefault();
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
        public static Task<SendSmsResult> SendAsync(string subject, string tag, string content
            , object parameters, params string[] mobiles)
        {
            return SendAsync(new SendSmsParameter()
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
        public static Task<SendSmsResult> SendAsync(string tag, string content, object parameters, params string[] mobiles)
        {
            return SendAsync(string.Empty, tag, content, parameters, mobiles);
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="content">Sms content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobile numbers</param>
        /// <returns></returns>
        public static Task<SendSmsResult> SendAsync(string content, object parameters, params string[] mobiles)
        {
            return SendAsync(string.Empty, DefaultTag, content, parameters, mobiles);
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Send sms parameter</param>
        /// <returns></returns>
        public static Task<SendSmsResult> SendAsync(SmsAccount account, SendSmsParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));
            SixnetDirectThrower.ThrowArgNullIf(parameter != null, nameof(parameter));

            var smsProvider = GetSmsProvider();
            return ExecuteAsync(account, parameter, smsProvider.SendAsync);
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="content">Content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobiles</param>
        /// <returns></returns>
        public static Task<SendSmsResult> SendAsync(SmsAccount account, string content, object parameters, params string[] mobiles)
        {
            return SendAsync(account, new SendSmsParameter()
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
        public static Task<ReceiveSmsResult> ReceiveAsync(ReceiveSmsParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return ExecuteAsync(smsAccount, parameter, smsProvider.ReceiveAsync);
        }

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Receive sms parameter</param>
        /// <returns>Return receive sms result</returns>
        public static Task<ReceiveSmsResult> ReceiveAsync(SmsAccount account, ReceiveSmsParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return ExecuteAsync(account, parameter, smsProvider.ReceiveAsync);
        }

        #endregion

        #region Get sms report

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="parameter">Get sms report parameter</param>
        /// <returns>Return sms report result</returns>
        public static Task<GetSmsReportResult> GetSmsReportAsync(GetSmsReportParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return ExecuteAsync(smsAccount, parameter, smsProvider.GetSmsReportAsync);
        }

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Get sms report parameter</param>
        /// <returns>Return sms report result</returns>
        public static Task<GetSmsReportResult> GetSmsReportAsync(SmsAccount account, GetSmsReportParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return ExecuteAsync(account, parameter, smsProvider.GetSmsReportAsync);
        }

        #endregion

        #region Query balance

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="parameter">Query balance parameter</param>
        /// <returns>Return query balance result</returns>
        public static Task<QuerySmsBalanceResult> QueryBalanceAsync(QueryBalanceParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return ExecuteAsync(smsAccount, parameter, smsProvider.QueryBalanceAsync);
        }

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Query balance parameter</param>
        /// <returns>Return query balance result</returns>
        public static Task<QuerySmsBalanceResult> QueryBalanceAsync(SmsAccount account, QueryBalanceParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return ExecuteAsync(account, parameter, smsProvider.QueryBalanceAsync);
        }

        #endregion

        #region Check black

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="parameter">Check black parameter</param>
        /// <returns>Return check black result</returns>
        public static Task<CheckBlackResult> CheckBlackAsync(CheckBlackParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return ExecuteAsync(smsAccount, parameter, smsProvider.CheckBlackAsync);
        }

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="parameter">Check black parameter</param>
        /// <returns>Return check black result</returns>
        public static Task<CheckBlackResult> CheckBlackAsync(SmsAccount account, CheckBlackParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return ExecuteAsync(account, parameter, smsProvider.CheckBlackAsync);
        }

        #endregion

        #region Check keyword

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="parameter">Check keyword parameter</param>
        /// <returns>Return check keyword result</returns>
        public static Task<CheckKeywordResult> CheckKeywordAsync(CheckKeywordParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            var smsOptions = GetSmsOptions();
            var smsAccount = GetSmsAccount(smsOptions, parameter);
            return ExecuteAsync(smsAccount, parameter, smsProvider.CheckKeywordAsync);
        }

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        public static Task<CheckKeywordResult> CheckKeywordAsync(SmsAccount account, CheckKeywordParameter parameter)
        {
            var smsProvider = GetSmsProvider();
            return ExecuteAsync(account, parameter, smsProvider.CheckKeywordAsync);
        }

        #endregion

        #region Util

        /// <summary>
        /// Execute sms operation
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="smsAccount"></param>
        /// <param name="smsParameter"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        internal static async Task<TResult> ExecuteAsync<TParameter, TResult>(SmsAccount smsAccount, TParameter smsParameter
            , Func<SmsAccount, TParameter, Task<TResult>> proxy) where TParameter : SmsParameter where TResult : SmsResult
        {
            var result = await proxy(smsAccount, smsParameter).ConfigureAwait(false);
            if (result != null)
            {
                result.SmsAccount = smsAccount;
                result.SmsOptions = smsParameter;
            }
            return result;
        }

        #endregion
    }
}
