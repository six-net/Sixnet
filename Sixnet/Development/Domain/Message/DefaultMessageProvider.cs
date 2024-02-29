using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Default message provider
    /// </summary>
    public class DefaultMessageProvider : ISixnetMessageProvider
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="options">Send message options</param>
        /// <returns>Return send result</returns>
        public SendMessageResult Send(SendMessageOptions options)
        {
            return SendAsync(options).Result;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="options">Send message options</param>
        /// <returns>Return send result</returns>
        public async Task<SendMessageResult> SendAsync(SendMessageOptions options)
        {
            if (options?.Messages.IsNullOrEmpty() ?? true)
            {
                return SendMessageResult.NoOptions();
            }
            var messageGroups = new Dictionary<string, List<SixnetMessageInfo>>();
            foreach (var msg in options.Messages)
            {
                if (messageGroups.ContainsKey(msg.Type))
                {
                    messageGroups[msg.Type].Add(msg);
                }
                else
                {
                    messageGroups.Add(msg.Type, new List<SixnetMessageInfo>()
                    {
                        msg
                    });
                }
            }
            var sendResult = SendMessageResult.SendSuccess();
            foreach (var msgGroup in messageGroups)
            {
                var messageHandler = SixnetMessager.GetMessageHandler(msgGroup.Key);
                if (messageHandler == null)
                {
                    return SendMessageResult.NoHandler(string.Format("Not set handler for {0}", msgGroup.Key));
                }
                var executeResult = await messageHandler.SendAsync(new SendMessageContext()
                {
                    Messages = msgGroup.Value
                }).ConfigureAwait(false);
                if (!executeResult.Success)
                {
                    return executeResult;
                }
                sendResult.Add(executeResult);
            }
            return sendResult;
        }
    }
}
