﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Natsuri.Module;
using Natsuri.Database;

namespace Natsuri
{
    public class Program
    {
        public static void Main(string[] args)
                  => new Program().MainAsync().GetAwaiter().GetResult();

        public static DiscordSocketClient Client { private set; get; }
        private readonly CommandService _commands = new CommandService();

        public static Db Db;
        public static Random Rand = new Random();

        public static DateTime StartTime { private set; get; }

        private Program()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
            });
            Client.Log += Utils.Log;
            _commands.Log += Utils.LogErrorAsync;
        }

        private async Task MainAsync()
        {
            var json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("Keys/Credentials.json"));
            if (json["botToken"] == null)
                throw new NullReferenceException("Invalid Credentials file");

            Db = new Db();
            await Db.InitAsync("Natsuri");

            await _commands.AddModuleAsync<Communication>(null);
            await _commands.AddModuleAsync<Information>(null);
            await _commands.AddModuleAsync<Entertainment>(null);

            Client.MessageReceived += HandleCommandAsync;
            Client.GuildAvailable += GuildJoined;
            Client.JoinedGuild += GuildJoined;

            StartTime = DateTime.Now;
            await Client.LoginAsync(TokenType.Bot, json["botToken"].Value<string>());
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task GuildJoined(SocketGuild guild)
        {
            await Db.InitGuildAsync(guild);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage msg = arg as SocketUserMessage;
            if (msg == null) return;
            int pos = 0;
            if (!arg.Author.IsBot && (msg.HasMentionPrefix(Client.CurrentUser, ref pos) || msg.HasStringPrefix("n.", ref pos)))
            {
                SocketCommandContext context = new SocketCommandContext(Client, msg);
                var result = await _commands.ExecuteAsync(context, pos, null);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.Error.ToString() + ": " + result.ErrorReason);
                }
            }
            await Db.AddMessageAsync(msg);
        }
    }
}
