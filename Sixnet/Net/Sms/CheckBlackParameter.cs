// "Company © 2025. All rights reserved."

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Check black parameter
    /// </summary>
    public class CheckBlackParameter : SmsParameter
    {
        public override SmsParameter Clone()
        {
            return new CheckBlackParameter()
            {
                Tag = Tag,
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
            };
        }
    }
}
