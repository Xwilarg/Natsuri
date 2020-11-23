using Discord;
using System;

namespace Natsuri.Impl
{
    [Serializable]
    public class Message
    {
        public static Message CreateFromMessage(IMessage message)
        {
            return new Message()
            {
                MessageId = message.Id.ToString(),
                MessageCreated = message.CreatedAt,
                Content = message.Content,
                ChannelId = message.Channel.Id.ToString(),
                IsChannelNsfw = (message.Channel as ITextChannel)?.IsNsfw
            };
        }

        public string MessageId;
        public DateTimeOffset MessageCreated;
        public string Content;
        public string ChannelId;
        public bool? IsChannelNsfw;
    }
}
