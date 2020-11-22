using Discord.Commands;
using System.Threading.Tasks;

namespace Natsuri.Module
{
    public class Information : ModuleBase
    {
        [Command("Stat")]
        public async Task Stat()
        {
            await ReplyAsync($"Your profile takes {(await Program.Db.GetSizeInDbAsync(Context.Guild.Id, Context.User.Id) / 1000000).ToString("0.00")} MB in the database.");
        }
    }
}
