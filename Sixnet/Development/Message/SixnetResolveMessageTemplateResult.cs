// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Message
{
    public struct SixnetResolveMessageTemplateResult
    {
        public bool Success { get; set; }

        public string ErrorParameterName { get; set; }

        public string NewContent { get; set; }

        public static SixnetResolveMessageTemplateResult Create(bool success, string errorParameterName = "", string newContent = "")
        {
            return new SixnetResolveMessageTemplateResult { Success = success, ErrorParameterName = errorParameterName, NewContent = newContent };
        }
    }
}
