namespace Natsuri.Impl
{
    public class Guild
    {
        public Guild(string id)
        {
            this.id = id;
        }

        public string id;
        public User[] Users = new User[0];
    }
}
