﻿using System;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Receive sms info
    /// </summary>
    public class ReceiveSmsInfo
    {
        /// <summary>
        /// Gets or sets the mobile number
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the subcode
        /// </summary>
        public string Subcode { get; set; }

        /// <summary>
        /// Gets or sets the receive time
        /// </summary>
        public DateTimeOffset ReceiveTime { get; set; }
    }
}
