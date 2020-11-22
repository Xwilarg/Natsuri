namespace Natsuri.Impl
{
    public class User
    {
        public User(string id)
        {
            this.id = id;
        }

        public string id;
        public Message[] Messages = new Message[0];
    }
}
