﻿using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Configuration;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms options
    /// </summary>
    public abstract class SmsOptions : IParameterOptions
    {
        /// <summary>
        /// Gets or sets the tag
        /// </summary>
        public string Tag { get; set; } = SmsManager.DefaultTag;

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        public abstract SmsOptions Clone();
    }
}
