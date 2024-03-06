using System.Threading.Tasks;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms provider contract
    /// </summary>
    public interface ISixnetSmsProvider
    {
        #region Receive sms

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Receive sms parameter</param>
        /// <returns>Return receive sms result</returns>
        ReceiveSmsResult Receive(SmsAccount account, ReceiveSmsParameter parameter);

        /// <summary>
        /// Receive sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Receive sms parameter</param>
        /// <returns>Return receive sms result</returns>
        Task<ReceiveSmsResult> ReceiveAsync(SmsAccount account, ReceiveSmsParameter parameter);

        #endregion

        #region Send sms

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Send sms parameter</param>
        /// <returns>Return send sms result</returns>
        SendSmsResult Send(SmsAccount account, SendSmsParameter parameter);

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Send sms parameter</param>
        /// <returns>Return send sms result</returns>
        Task<SendSmsResult> SendAsync(SmsAccount account, SendSmsParameter parameter);

        #endregion

        #region Get sms report

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Get sms report parameter</param>
        /// <returns>Return sms report result</returns>
        GetSmsReportResult GetSmsReport(SmsAccount account, GetSmsReportParameter parameter);

        /// <summary>
        /// Get sms report
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Get sms report parameter</param>
        /// <returns>Return sms report result</returns>
        Task<GetSmsReportResult> GetSmsReportAsync(SmsAccount account, GetSmsReportParameter parameter);

        #endregion

        #region Query balance

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Query balance parameter</param>
        /// <returns>Return query balance result</returns>
        QuerySmsBalanceResult QueryBalance(SmsAccount account, QueryBalanceParameter parameter);

        /// <summary>
        /// Query balance
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Query balance parameter</param>
        /// <returns>Return query balance result</returns>
        Task<QuerySmsBalanceResult> QueryBalanceAsync(SmsAccount account, QueryBalanceParameter parameter);

        #endregion

        #region Check black

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="parameter">Check black parameter</param>
        /// <returns>Return check black result</returns>
        CheckBlackResult CheckBlack(SmsAccount account, CheckBlackParameter parameter);

        /// <summary>
        /// Check black
        /// </summary>
        /// <param name="account">Sms acount</param>
        /// <param name="parameter">Check black parameter</param>
        /// <returns>Return check black result</returns>
        Task<CheckBlackResult> CheckBlackAsync(SmsAccount account, CheckBlackParameter parameter);

        #endregion

        #region Check keyword

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Check keyword parameter</param>
        /// <returns>Return check keyword result</returns>
        CheckKeywordResult CheckKeyword(SmsAccount account, CheckKeywordParameter parameter);

        /// <summary>
        /// Check keyword
        /// </summary>
        /// <param name="account">Sms account</param>
        /// <param name="parameter">Check keyword parameter</param>
        /// <returns>Return check keyword result</returns>
        Task<CheckKeywordResult> CheckKeywordAsync(SmsAccount account, CheckKeywordParameter parameter);

        #endregion
    }
}
