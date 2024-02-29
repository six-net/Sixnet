using Sixnet.MQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Sixnet.Net.Sms
{
    public static partial class SixnetSms
    {
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
                InProcessQueueSmsMessage smsInternalMessageCommand = new InProcessQueueSmsMessage()
                {
                    SmsOptions = sendSmsOptions
                };
                InProcessQueueManager.GetQueue(SixnetMQ.InProcessQueueNames.Message).Enqueue(smsInternalMessageCommand);
                return SmsResult.SendSuccess<SendSmsResult>();
            }
            else
            {
                var smsProvider = GetSmsProvider();
                var sendResult = await ExecuteAsync(sendSmsOptions, smsProvider.SendAsync).ConfigureAwait(false);
                //callback
                ThreadPool.QueueUserWorkItem(s =>
                {
                    _smsOptions.SendCallback?.Invoke(new SendSmsResult[1] { sendResult.Clone() });
                });
                return sendResult;
            }
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
                Parameters = parameters.ToStringDictionary()
            }).ConfigureAwait(false);
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
                InProcessQueueSmsMessage smsInternalMessageCommand = new InProcessQueueSmsMessage()
                {
                    SmsOptions = sendSmsOptions,
                    SmsAccount = account
                };
                InProcessQueueManager.GetQueue(SixnetMQ.InProcessQueueNames.Message).Enqueue(smsInternalMessageCommand);
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
                Parameters = parameters?.ToStringDictionary(),
                Asynchronously = asynchronously,
                Mobiles = mobiles
            }).ConfigureAwait(false);
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
        /// <param name="account">Sms account</param>
        /// <param name="receiveSmsOptions">Receive sms options</param>
        /// <returns>Return receive sms result</returns>
        public static async Task<ReceiveSmsResult> ReceiveAsync(SmsAccount account, ReceiveSmsOptions receiveSmsOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, receiveSmsOptions, smsProvider.ReceiveAsync).ConfigureAwait(false);
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
        /// <param name="account">Sms account</param>
        /// <param name="getSmsReportOptions">Get sms report options</param>
        /// <returns>Return sms report result</returns>
        public static async Task<GetSmsReportResult> GetSmsReportAsync(SmsAccount account, GetSmsReportOptions getSmsReportOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, getSmsReportOptions, smsProvider.GetSmsReportAsync).ConfigureAwait(false);
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
        /// <param name="account">Sms account</param>
        /// <param name="queryBalanceOptions">Query balance options</param>
        /// <returns>Return query balance result</returns>
        public static async Task<QuerySmsBalanceResult> QueryBalanceAsync(SmsAccount account, QueryBalanceOptions queryBalanceOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, queryBalanceOptions, smsProvider.QueryBalanceAsync).ConfigureAwait(false);
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
        /// <param name="account">Sms acount</param>
        /// <param name="checkBlackOptions">Check black options</param>
        /// <returns>Return check black result</returns>
        public static async Task<CheckBlackResult> CheckBlackAsync(SmsAccount account, CheckBlackOptions checkBlackOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, checkBlackOptions, smsProvider.CheckBlackAsync).ConfigureAwait(false);
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
        /// <param name="account">Sms account</param>
        /// <param name="checkKeywordOptions">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        public static async Task<CheckKeywordResult> CheckKeywordAsync(SmsAccount account, CheckKeywordOptions checkKeywordOptions)
        {
            var smsProvider = GetSmsProvider();
            return await ExecuteAsync(account, checkKeywordOptions, smsProvider.CheckKeywordAsync).ConfigureAwait(false);
        }

        #endregion
    }
}
