using Discord.WebSocket;
using Natsuri.Impl;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System.Collections.Generic;
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

        public void InitGuild(SocketGuild sGuild)
        {
            if (_guilds.ContainsKey(sGuild.Id))
                return;

            _guilds.Add(sGuild.Id, new Guild(sGuild.Id.ToString()));
        }

        public Dictionary<ulong, Guild> _guilds = new Dictionary<ulong, Guild>();

        private readonly RethinkDB _r;
        private Connection _conn;
        private string _dbName;
    }
}
