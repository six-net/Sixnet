using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.Model;
using System;
using System.Threading.Tasks;

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
        readonly static SmsOptions _smsOptions = new SmsOptions();

        #endregion

        #region Configure

        /// <summary>
        /// Configure sms
        /// </summary>
        /// <param name="configure"></param>
        public static void Configure(Action<SmsOptions> configure)
        {
            configure?.Invoke(_smsOptions);
        }

        #endregion

        #region Send sms

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="sendSmsOptions">Send sms options</param>
        /// <returns>Return send sms result</returns>
        public static SendSmsResult Send(SendSmsOptions sendSmsOptions)
        {
            return SendAsync(sendSmsOptions).Result;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="tag">Sms tag</param>
        /// <param name="content">Content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="mobiles">Receive mobiles</param>
        /// <returns>Return sms send result</returns>
        public static SendSmsResult Send(string tag, string content, object parameters, bool asynchronously = true, params string[] mobiles)
        {
            return SendAsync(tag, content, parameters, asynchronously, mobiles).Result;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="content">Sms content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="mobiles">Mobile numbers</param>
        /// <returns>Return the send sms result</returns>
        public static SendSmsResult Send(string content, object parameters, bool asynchronously = true, params string[] mobiles)
        {
            return SendAsync(content, parameters, asynchronously, mobiles).Result;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="sendSmsOptions">Send sms options</param>
        /// <returns>Return send sms result</returns>
        public static SendSmsResult Send(SmsAccount account, SendSmsOptions sendSmsOptions)
        {
            return SendAsync(account, sendSmsOptions).Result;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="content">Content</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobile numbers</param>
        /// <returns>Return send result</returns>
        public static SendSmsResult Send(SmsAccount smsAccount, string content, object parameters, bool asynchronously = true, params string[] mobiles)
        {
            return SendAsync(smsAccount, content, parameters, asynchronously, mobiles).Result;
        }

        #endregion

        #region Receive sms

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="receiveSmsOptions">Receive sms options</param>
        /// <returns>Return receive sms result</returns>
        public static ReceiveSmsResult Receive(ReceiveSmsOptions receiveSmsOptions)
        {
            return ReceiveAsync(receiveSmsOptions).Result;
        }

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="receiveSmsOptions">Receive sms options</param>
        /// <returns>Return receive sms result</returns>
        public static ReceiveSmsResult Receive(SmsAccount account, ReceiveSmsOptions receiveSmsOptions)
        {
            return ReceiveAsync(account, receiveSmsOptions).Result;
        }

        #endregion

        #region Get sms report

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="getSmsReportOptions">Get sms report options</param>
        /// <returns>Return sms report result</returns>
        public static GetSmsReportResult GetSmsReport(GetSmsReportOptions getSmsReportOptions)
        {
            return GetSmsReportAsync(getSmsReportOptions).Result;
        }

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="getSmsReportOptions">Get sms report options</param>
        /// <returns>Return sms report result</returns>
        public static GetSmsReportResult GetSmsReport(SmsAccount account, GetSmsReportOptions getSmsReportOptions)
        {
            return GetSmsReportAsync(account, getSmsReportOptions).Result;
        }

        #endregion

        #region Query balance

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="queryBalanceOptions">Query balance options</param>
        /// <returns>Return query balance result</returns>
        public static QuerySmsBalanceResult QueryBalance(QueryBalanceOptions queryBalanceOptions)
        {
            return QueryBalanceAsync(queryBalanceOptions).Result;
        }

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="queryBalanceOptions">Query balance options</param>
        /// <returns>Return query balance result</returns>
        public static QuerySmsBalanceResult QueryBalance(SmsAccount account, QueryBalanceOptions queryBalanceOptions)
        {
            return QueryBalanceAsync(account, queryBalanceOptions).Result;
        }

        #endregion

        #region Check black

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="checkBlackOptions">Check black options</param>
        /// <returns>Return check black result</returns>
        public static CheckBlackResult CheckBlack(CheckBlackOptions checkBlackOptions)
        {
            return CheckBlackAsync(checkBlackOptions).Result;
        }

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="checkBlackOptions">Check black options</param>
        /// <returns>Return check black result</returns>
        public static CheckBlackResult CheckBlack(SmsAccount account, CheckBlackOptions checkBlackOptions)
        {
            return CheckBlackAsync(account, checkBlackOptions).Result;
        }

        #endregion

        #region Check keyword

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="checkKeywordOptions">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        public static CheckKeywordResult CheckKeyword(CheckKeywordOptions checkKeywordOptions)
        {
            return CheckKeywordAsync(checkKeywordOptions).Result;
        }

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="checkKeywordOptions">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        public static CheckKeywordResult CheckKeyword(SmsAccount account, CheckKeywordOptions checkKeywordOptions)
        {
            return CheckKeywordAsync(account, checkKeywordOptions).Result;
        }

        #endregion

        #region Util

        static ISixnetSmsProvider GetSmsProvider()
        {
            var smsProvider = SixnetContainer.GetService<ISixnetSmsProvider>();
            SixnetDirectThrower.ThrowSixnetExceptionIf(smsProvider == null, "Not config sms provider");
            return smsProvider;
        }

        internal static async Task<TResult> ExecuteAsync<TOptions, TResult>(TOptions smsOptions, Func<SmsAccount, TOptions, Task<TResult>> proxy) where TOptions : SmsExecutionOptions where TResult : SmsResult
        {
            var smsAccount = GetSmsAccount(smsOptions);
            if (smsAccount == null)
            {
                throw new SixnetException("No sms account specified");
            }
            var result = await proxy(smsAccount, smsOptions).ConfigureAwait(false);
            if (result != null)
            {
                result.SmsAccount = smsAccount;
                result.SmsOptions = smsOptions;
            }
            return result;
        }

        internal static Task<TResult> ExecuteAsync<TOptions, TResult>(SmsAccount account, TOptions smsOptions, Func<SmsAccount, TOptions, Task<TResult>> proxy) where TOptions : SmsExecutionOptions where TResult : SmsResult
        {
            if (account == null)
            {
                throw new SixnetException("No sms account specified");
            }
            return proxy(account, smsOptions);
        }

        static void SetAdditions(SmsExecutionOptions smsOptions)
        {
            if (smsOptions != null)
            {
                //work id
                smsOptions.AddWorkId();
            }
        }

        /// <summary>
        /// Get sms account
        /// </summary>
        /// <param name="smsOptions">Sms options</param>
        /// <returns></returns>
        static SmsAccount GetSmsAccount(SmsExecutionOptions smsOptions)
        {
            return _smsOptions?.GetSmsAccountFunc?.Invoke(smsOptions);
        }

        #endregion
    }
}
