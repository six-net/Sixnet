using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Message
{
    public struct ResolveMessageTemplateResult
    {
        public bool Success { get; set; }

        public string ErrorParameterName { get; set; }

        public string NewContent { get; set; }

        public static ResolveMessageTemplateResult Create(bool success, string errorParameterName = "", string newContent = "")
        {
            return new ResolveMessageTemplateResult { Success = success, ErrorParameterName = errorParameterName, NewContent = newContent };
        }
    }
}
