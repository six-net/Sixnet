// "Company © 2025. All rights reserved."

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Query balance parameter
    /// </summary>
    public class QueryBalanceParameter : SmsParameter
    {
        public override SmsParameter Clone()
        {
            return new QueryBalanceParameter()
            {
                Tag = Tag,
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
