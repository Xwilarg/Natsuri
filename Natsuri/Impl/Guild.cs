using System.Collections.Generic;

namespace Natsuri.Impl
{
    public class Guild
    {
        public Guild(string id)
        {
            this.id = id;
        }

        public string id;
        public Dictionary<string, User> Users = new Dictionary<string, User>();
    }
}
