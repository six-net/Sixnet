using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Sms
{
    /// <summary>
    /// Defines the sms send status
    /// </summary>
    public enum SmsSendStatus
    {
        Success = 0,
        Fail = 1000,
        WaitReceipt = 2000
    }

    /// <summary>
    /// Defines the sms format
    /// </summary>
    public enum SmsContentFormat
    {
        JSON = 10,
        XML = 20,
        Text = 30
    }
}
