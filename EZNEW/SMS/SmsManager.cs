using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.DependencyInjection;
using EZNEW.Diagnostics;
using EZNEW.Fault;
using EZNEW.Queue;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms manager
    /// </summary>
    public static class SmsManager
    {
        #region Properties

        /// <summary>
        /// Get sms provider proxy
        /// </summary>
        static readonly Func<ISmsProvider> GetSmsProviderProxy = () => ContainerManager.Resolve<ISmsProvider>();

        /// <summary>
        /// Get sms account proxy
        /// </summary>
        static Func<SmsOptions, SmsAccount> GetSmsAccountProxy = null;

        /// <summary>
        /// Gets the default tag
        /// </summary>
        public const string DefaultTag = "EZNEW_SMS_DEFAULT_TAG";

        /// <summary>
        /// Sms sent callback
        /// </summary>
        static Action<IEnumerable<SendSmsResult>> SmsSentCallback;

        #endregion

        #region Sms account

        /// <summary>
        /// Configure get sms account proxy
        /// </summary>
        /// <param name="getSmsAccountProxy"></param>
        public static void ConfigureSmsAccount(Func<SmsOptions, SmsAccount> getSmsAccountProxy)
        {
            GetSmsAccountProxy = getSmsAccountProxy;
        }

        /// <summary>
        /// Get sms account
        /// </summary>
        /// <param name="smsOptions">Sms options</param>
        /// <returns></returns>
        public static SmsAccount GetSmsAccount(SmsOptions smsOptions)
        {
            return GetSmsAccountProxy?.Invoke(smsOptions);
        }

        #endregion

        #region Receive sms

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="receiveSmsOptions">Receive sms options</param>
        /// <returns>Return receive sms result</returns>
        public static async Task<ReceiveSmsResult> ReceiveAsync(ReceiveSmsOptions receiveSmsOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(receiveSmsOptions, smsProvider.ReceiveAsync).ConfigureAwait(false);
        }

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
        public static async Task<ReceiveSmsResult> ReceiveAsync(SmsAccount account, ReceiveSmsOptions receiveSmsOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, receiveSmsOptions, smsProvider.ReceiveAsync).ConfigureAwait(false);
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

        #region Send sms

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="sendSmsOptions">Sms options</param>
        /// <returns>Return send result</returns>
        public static async Task<SendSmsResult> SendAsync(SendSmsOptions sendSmsOptions)
        {
            if (sendSmsOptions == null)
            {
                throw new ArgumentNullException(nameof(sendSmsOptions));
            }
            //Set additions
            SetAdditions(sendSmsOptions);
            if (sendSmsOptions.Asynchronously)
            {
                InternalQueueSendSmsItem smsInternalMessageCommand = new InternalQueueSendSmsItem()
                {
                    SmsOptions = sendSmsOptions
                };
                InternalQueueManager.GetQueue(EZNEWConstants.InternalQueueNames.Message).Enqueue(smsInternalMessageCommand);
                return SmsResult.SendSuccess<SendSmsResult>();
            }
            else
            {
                var smsProvider = GetSmsProvider();
                var sendResult = await ExecuteAsync(sendSmsOptions, smsProvider.SendAsync).ConfigureAwait(false);
                //callback
                ThreadPool.QueueUserWorkItem(s =>
                {
                    SmsSentCallback?.Invoke(new SendSmsResult[1] { sendResult.Clone() });
                });
                return sendResult;
            }
        }

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
        public static async Task<SendSmsResult> SendAsync(string tag, string content, object parameters, bool asynchronously = true, params string[] mobiles)
        {
            return await SendAsync(new SendSmsOptions()
            {
                Asynchronously = asynchronously,
                Mobiles = mobiles,
                Tag = tag,
                Content = content,
                Parameters = parameters.ObjectToStringDcitionary()
            }).ConfigureAwait(false);
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
        public static async Task<SendSmsResult> SendAsync(string content, object parameters, bool asynchronously = true, params string[] mobiles)
        {
            return await SendAsync(DefaultTag, content, parameters, asynchronously, mobiles).ConfigureAwait(false);
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
        public static async Task<SendSmsResult> SendAsync(SmsAccount account, SendSmsOptions sendSmsOptions)
        {
            if (sendSmsOptions == null)
            {
                throw new ArgumentNullException(nameof(sendSmsOptions));
            }
            if (sendSmsOptions.Asynchronously)
            {
                InternalQueueSendSmsItem smsInternalMessageCommand = new InternalQueueSendSmsItem()
                {
                    SmsOptions = sendSmsOptions,
                    SmsAccount = account
                };
                InternalQueueManager.GetQueue(EZNEWConstants.InternalQueueNames.Message).Enqueue(smsInternalMessageCommand);
                return SmsResult.SendSuccess<SendSmsResult>();
            }
            else
            {
                var smsProvider = GetSmsProvider();
                return await ExecuteAsync(account, sendSmsOptions, smsProvider.SendAsync).ConfigureAwait(false);
            }
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
        public static async Task<SendSmsResult> SendAsync(SmsAccount smsAccount, string content, object parameters, bool asynchronously = true, params string[] mobiles)
        {
            return await SendAsync(smsAccount, new SendSmsOptions()
            {
                Content = content,
                Parameters = parameters?.ObjectToStringDcitionary(),
                Asynchronously = asynchronously,
                Mobiles = mobiles
            }).ConfigureAwait(false);
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

        #region Get sms report

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="getSmsReportOptions">Get sms report options</param>
        /// <returns>Return sms report result</returns>
        public static async Task<GetSmsReportResult> GetSmsReportAsync(GetSmsReportOptions getSmsReportOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(getSmsReportOptions, smsProvider.GetSmsReportAsync).ConfigureAwait(false);
        }

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
        public static async Task<GetSmsReportResult> GetSmsReportAsync(SmsAccount account, GetSmsReportOptions getSmsReportOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, getSmsReportOptions, smsProvider.GetSmsReportAsync).ConfigureAwait(false);
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
        public static async Task<QuerySmsBalanceResult> QueryBalanceAsync(QueryBalanceOptions queryBalanceOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(queryBalanceOptions, smsProvider.QueryBalanceAsync).ConfigureAwait(false);
        }

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
        public static async Task<QuerySmsBalanceResult> QueryBalanceAsync(SmsAccount account, QueryBalanceOptions queryBalanceOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, queryBalanceOptions, smsProvider.QueryBalanceAsync).ConfigureAwait(false);
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
        public static async Task<CheckBlackResult> CheckBlackAsync(CheckBlackOptions checkBlackOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(checkBlackOptions, smsProvider.CheckBlackAsync).ConfigureAwait(false);
        }

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
        public static async Task<CheckBlackResult> CheckBlackAsync(SmsAccount account, CheckBlackOptions checkBlackOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, checkBlackOptions, smsProvider.CheckBlackAsync).ConfigureAwait(false);
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
        public static async Task<CheckKeywordResult> CheckKeywordAsync(CheckKeywordOptions checkKeywordOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(checkKeywordOptions, smsProvider.CheckKeywordAsync).ConfigureAwait(false);
        }

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
        public static async Task<CheckKeywordResult> CheckKeywordAsync(SmsAccount account, CheckKeywordOptions checkKeywordOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, checkKeywordOptions, smsProvider.CheckKeywordAsync).ConfigureAwait(false);
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

        #region Callback

        /// <summary>
        /// Add send sms callback
        /// </summary>
        /// <param name="callbackAction">Callback action</param>
        public static void AddSmsSentCallback(Action<IEnumerable<SendSmsResult>> callbackAction)
        {
            if (callbackAction == null)
            {
                return;
            }
            if (SmsSentCallback == null)
            {
                SmsSentCallback = callbackAction;
            }
            else
            {
                SmsSentCallback += callbackAction;
            }
        }

        #endregion

        #region Util

        static ISmsProvider GetSmsProvider()
        {
            var smsProvider = GetSmsProviderProxy();
            if (smsProvider == null)
            {
                throw new EZNEWException("No sms provider specified");
            }
            return smsProvider;
        }

        internal static async Task<TResult> ExecuteAsync<TOptions, TResult>(TOptions smsOptions, Func<SmsAccount, TOptions, Task<TResult>> proxy) where TOptions : SmsOptions where TResult : SmsResult
        {
            var smsAccount = GetSmsAccount(smsOptions);
            if (smsAccount == null)
            {
                throw new EZNEWException("No sms account specified");
            }
            var result = await proxy(smsAccount, smsOptions).ConfigureAwait(false);
            if (result != null)
            {
                result.SmsAccount = smsAccount;
                result.SmsOptions = smsOptions;
            }
            return result;
        }

        internal static Task<TResult> ExecuteAsync<TOptions, TResult>(SmsAccount account, TOptions smsOptions, Func<SmsAccount, TOptions, Task<TResult>> proxy) where TOptions : SmsOptions where TResult : SmsResult
        {
            if (account == null)
            {
                throw new EZNEWException("No sms account specified");
            }
            return proxy(account, smsOptions);
        }

        static void SetAdditions(SmsOptions smsOptions)
        {
            if (smsOptions != null)
            {
                //work id
                smsOptions.AddWorkId();
            }
        }

        #endregion
    }
}
