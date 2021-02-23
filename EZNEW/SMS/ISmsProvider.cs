using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms provider contract
    /// </summary>
    public interface ISmsProvider
    {
        #region Receive sms

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="receiveSmsOptions">Receive sms options</param>
        /// <returns>Return receive sms result</returns>
        ReceiveSmsResult Receive(SmsAccount account, ReceiveSmsOptions receiveSmsOptions);

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="receiveSmsOptions">Receive sms options</param>
        /// <returns>Return receive sms result</returns>
        Task<ReceiveSmsResult> ReceiveAsync(SmsAccount account, ReceiveSmsOptions receiveSmsOptions);

        #endregion

        #region Send sms

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="sendSmsOptions">Send sms options</param>
        /// <returns>Return send sms result</returns>
        SendSmsResult Send(SmsAccount account, SendSmsOptions sendSmsOptions);

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="sendSmsOptions">Send sms options</param>
        /// <returns>Return send sms result</returns>
        Task<SendSmsResult> SendAsync(SmsAccount account, SendSmsOptions sendSmsOptions);

        #endregion

        #region Get sms report

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="getSmsReportOptions">Get sms report options</param>
        /// <returns>Return sms report result</returns>
        GetSmsReportResult GetSmsReport(SmsAccount account, GetSmsReportOptions getSmsReportOptions);

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="getSmsReportOptions">Get sms report options</param>
        /// <returns>Return sms report result</returns>
        Task<GetSmsReportResult> GetSmsReportAsync(SmsAccount account, GetSmsReportOptions getSmsReportOptions);

        #endregion

        #region Query balance

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="queryBalanceOptions">Query balance options</param>
        /// <returns>Return query balance result</returns>
        QuerySmsBalanceResult QueryBalance(SmsAccount account, QueryBalanceOptions queryBalanceOptions);

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="queryBalanceOptions">Query balance options</param>
        /// <returns>Return query balance result</returns>
        Task<QuerySmsBalanceResult> QueryBalanceAsync(SmsAccount account, QueryBalanceOptions queryBalanceOptions);

        #endregion

        #region Check black

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="checkBlackOptions">Check black options</param>
        /// <returns>Return check black result</returns>
        CheckBlackResult CheckBlack(SmsAccount account, CheckBlackOptions checkBlackOptions);

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="checkBlackOptions">Check black options</param>
        /// <returns>Return check black result</returns>
        Task<CheckBlackResult> CheckBlackAsync(SmsAccount account, CheckBlackOptions checkBlackOptions);

        #endregion

        #region Check keyword

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="checkKeywordOptions">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        CheckKeywordResult CheckKeyword(SmsAccount account, CheckKeywordOptions checkKeywordOptions);

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="checkKeywordOptions">Check keyword options</param>
        /// <returns>Return check keyword result</returns>
        Task<CheckKeywordResult> CheckKeywordAsync(SmsAccount account, CheckKeywordOptions checkKeywordOptions);

        #endregion
    }
}
