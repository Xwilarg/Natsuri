using System;
using System.Collections.Generic;

namespace Natsuri.Impl
{
    [Serializable]
    public class User
    {
        public User(string id)
        {
            UserId = id;
        }

        public string UserId;
        public List<Message> Messages = new List<Message>();
        public List<Embed> Embeds = new List<Embed>();
    }
}
