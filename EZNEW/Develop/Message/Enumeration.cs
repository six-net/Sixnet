using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Defines message result type
    /// </summary>
    public enum MessageResultType
    {
        NoMessageProvider = 110,
        UnknownMessageType = 111,
        NoMessageTemplate = 112,
        SendOptionsIsNull = 113,
        NotSetTemplateParameterValue = 114,
        NoReceiver = 115,
        NoHandler = 116,
        MessageIsNullOrEmpty = 117,
        Failed = 1999,
        Success = 2000
    }
}
