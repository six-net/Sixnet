using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message time
    /// </summary>
    [Serializable]
    public enum MessageSendTime
    {
        Immediately = 2,
        WorkCompleted = 4
    }
}
