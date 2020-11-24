﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Natsuri.Impl;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Natsuri.Database
{
    public sealed class Db
    {
        public Db()
        {
            _r = RethinkDB.R;
        }

        public async Task InitAsync(string dbName)
        {
            _dbName = dbName;
            _conn = await _r.Connection().ConnectAsync();
            if (!await _r.DbList().Contains(_dbName).RunAsync<bool>(_conn))
                _r.DbCreate(_dbName).Run(_conn);
            if (!await _r.Db(_dbName).TableList().Contains("Guilds").RunAsync<bool>(_conn))
                _r.Db(_dbName).TableCreate("Guilds").Run(_conn);
        }

        public async Task InitGuildAsync(SocketGuild sGuild)
        {
            if (_guilds.ContainsKey(sGuild.Id))
                return;

            Guild guild;
            if (await _r.Db(_dbName).Table("Guilds").GetAll(sGuild.Id.ToString()).Count().Eq(0).RunAsync<bool>(_conn)) // Guild doesn't exist in db
            {
                guild = new Guild(sGuild.Id.ToString());
                foreach (var chan in sGuild.Channels)
                {
                    if (chan is ITextChannel textChan)
                    {
                        try
                        {
                            foreach (var msg in await textChan.GetMessagesAsync().FlattenAsync())
                                AddMessageInternal(guild, msg);
                        }
                        catch (HttpException)
                        { }
                    }
                }
                await _r.Db(_dbName).Table("Guilds").Insert(guild).RunAsync(_conn);
            }
            else
            {
                guild = await _r.Db(_dbName).Table("Guilds").Get(sGuild.Id.ToString()).RunAsync<Guild>(_conn);
            }
            _guilds.Add(sGuild.Id, guild);
        }

        public async Task AddMessageAsync(IMessage msg)
        {
            if (msg.Channel is ITextChannel chan)
            {
                var guild = _guilds[chan.GuildId];

                AddMessageInternal(guild, msg);

                await _r.Db(_dbName).Table("Guilds").Update(_r.HashMap("id", guild.id)
                    .With("Users", guild.Users)
                ).RunAsync(_conn);
            }
        }

        private void AddMessageInternal(Guild guild, IMessage msg)
        {
            User user;
            if (!guild.Users.ContainsKey(msg.Author.Id.ToString()))
            {
                user = new User(msg.Author.Id.ToString());
                guild.Users.Add(msg.Author.Id.ToString(), user);
            }
            else
                user = guild.Users[msg.Author.Id.ToString()];
            user.Messages.Add(Message.CreateFromMessage(msg));
            if (msg.Embeds.Count > 0)
                user.Embeds.Add(Impl.Embed.InitEmbed(msg.Embeds.ElementAt(0)));
        }

        public async Task<long> GetSizeInDbAsync(ulong guildId, ulong userId)
        {
            var g = (Guild)await _r.Db(_dbName).Table("Guilds").Get(guildId.ToString()).RunAsync<Guild>(_conn);
            if (g.Users.ContainsKey(userId.ToString()))
            {
                using (var s = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(s, g.Users[userId.ToString()]);
                    return s.Length;
                }
            }
            return 0L;
        }

        public List<Message> GetMessages(ulong guildId)
        {
            var all = _guilds[guildId].Users.Where(x => x.Value.UserId != Program.Client.CurrentUser.Id.ToString()).Select(x => x.Value.Messages);
            List<Message> allMessages = new List<Message>();
            foreach (var m in all)
            {
                allMessages.AddRange(m);
            }
            return allMessages;
        }

        public List<Message> GetMessages(ulong guildId, ulong userId)
        {
            return _guilds[guildId].Users[userId.ToString()].Messages;
        }

        public Dictionary<ulong, Guild> _guilds = new Dictionary<ulong, Guild>();

        private readonly RethinkDB _r;
        private Connection _conn;
        private string _dbName;
    }
}
