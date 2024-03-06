using Sixnet.Exceptions;
using Sixnet.Logging;
using Sixnet.Threading.Locking;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Default message provider
    /// </summary>
    internal class DefaultMessageProvider : ISixnetMessageProvider
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="parameter">Send message parameter</param>
        /// <returns>Return send result</returns>
        public void Send(SendMessageParameter parameter)
        {
            var messageGroup = GroupMessage(parameter);
            foreach (var msgGroupItem in messageGroup)
            {
                try
                {
                    var messageHandler = SixnetMessager.GetMessageHandler(msgGroupItem.Key);
                    messageHandler.Send(new SendMessageContext()
                    {
                        Messages = msgGroupItem.Value
                    });
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="options">Send message options</param>
        /// <returns>Return send result</returns>
        public async Task SendAsync(SendMessageParameter parameter)
        {
            var messageGroup = GroupMessage(parameter);
            foreach (var msgGroupItem in messageGroup)
            {
                try
                {
                    var messageHandler = SixnetMessager.GetMessageHandler(msgGroupItem.Key);
                    await messageHandler.SendAsync(new SendMessageContext()
                    {
                        Messages = msgGroupItem.Value
                    }).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SixnetLogger.LogError(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Group message
        /// Key => message type
        /// Value => subject entry
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Dictionary<string, List<SubjectMessageEntry>> GroupMessage(SendMessageParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.Messages.IsNullOrEmpty(), nameof(SendMessageParameter.Messages));

            var messageGroups = new Dictionary<string, List<SubjectMessageEntry>>();
            var messageOptions = SixnetMessager.GetMessageOptions();
            foreach (var msg in parameter.Messages)
            {
                if (string.IsNullOrWhiteSpace(msg.Subject))
                {
                    continue;
                }
                var supportMessageTypes = messageOptions.GetSupportMessageTypes(msg.Subject);
                if (supportMessageTypes.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var msgType in supportMessageTypes)
                {
                    if (messageGroups.ContainsKey(msgType))
                    {
                        var subjectMessageEntry = messageGroups[msgType].FirstOrDefault(c => c.Subject == msg.Subject);
                        if (subjectMessageEntry != null)
                        {
                            subjectMessageEntry.MessageInfos.Add(msg);
                        }
                        else
                        {
                            messageGroups[msgType].Add(new SubjectMessageEntry()
                            {
                                Subject = msg.Subject,
                                MessageInfos = new List<MessageInfo>() { msg }
                            });
                        }
                    }
                    else
                    {
                        messageGroups.Add(msgType, new List<SubjectMessageEntry>()
                        {
                            new()
                            {
                                Subject = msg.Subject,
                                MessageInfos = new List<MessageInfo>(){ msg }
                            }
                        });
                    }
                }
            }
            return messageGroups;
        }
    }
}
