using Discord;
using System;
using System.Linq;

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
                IsChannelNsfw = (message.Channel as ITextChannel)?.IsNsfw,
                Embed = message.Embeds.Count > 0 ? Embed.CreateFromEmbed(message.Embeds.ElementAt(0)) : null
            };
        }

        public string MessageId;
        public DateTimeOffset MessageCreated;
        public string Content;
        public string ChannelId;
        public bool? IsChannelNsfw;
        public Embed Embed;
    }
}
