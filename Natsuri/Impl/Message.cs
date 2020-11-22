using Discord;
using System;

namespace Natsuri.Impl
{
    public class Message
    {
        public Message(IMessage message)
        {
            id = message.Id.ToString();
            MessageCreated = message.CreatedAt;
            Content = message.Content;
        }

        public string id;
        public DateTimeOffset MessageCreated;
        public string Content;
    }
}
