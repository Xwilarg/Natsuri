using Discord.Commands;
using DiscordUtils;
using System.Threading.Tasks;

namespace Natsuri.Module
{
    public class Communication : ModuleBase
    {
        [Command("Info")]
        public async Task Info()
        {
            await ReplyAsync(embed: Utils.GetBotInfo(Program.StartTime, "Natsuri", Program.Client.CurrentUser));
        }
    }
}
