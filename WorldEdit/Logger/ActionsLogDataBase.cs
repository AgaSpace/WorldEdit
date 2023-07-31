using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;

using TShockAPI;
using static TShockAPI.TShock;
using TShockAPI.DB;

using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

using WorldEdit.Commands;

namespace WorldEdit.Logger
{
    public class ActionsLogDataBase
    {
        static readonly string name = "WorldEdit_History";
        private IDbConnection _db;

        public ActionsLogDataBase(IDbConnection db, SqlTableCreator creator)
        {
            _db = db;
            creator.EnsureTableStructure(new SqlTable(name, 
                new SqlColumn("AccountID", MySqlDbType.Int32),
                new SqlColumn("Action", MySqlDbType.Text),
                new SqlColumn("FullAction", MySqlDbType.Text),
                new SqlColumn("Time", MySqlDbType.DateTime) { Primary = true },
                new SqlColumn("X", MySqlDbType.Int32),
                new SqlColumn("Y", MySqlDbType.Int32),
                new SqlColumn("X2", MySqlDbType.Int32),
                new SqlColumn("Y2", MySqlDbType.Int32),
                new SqlColumn("WorldID", MySqlDbType.Int32)));
        }

        public bool Log(UserAccount account, WECommand command) => 
            Log(account, command, command.x, command.y, command.x2, command.y2);
        public bool Log(UserAccount account, WECommand command, int x, int y, int x2, int y2)
        {
            return _db.Query($"INSERT INTO {name} VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8)", account.ID, command.GetType().Name, command.action,
                DateTime.UtcNow, x, y, x2, y2, Main.worldID) > 0;
        }

        public IEnumerable<WorldEditAction> Log(int x, int y, int worldID)
        {
            using (QueryResult result = 
                //_db.QueryReader($"SELECT * FROM {name} WHERE WorldID = @0 AND X <= @1 AND X2 >= @1 AND Y <= @2 AND Y >= @2",
                _db.QueryReader($"SELECT * FROM {name} WHERE WorldID = @0 AND @1 >= X AND @1 <= X2 AND @2 >= Y AND @2 <= Y2",
                worldID, x, y))
            {
                while (result.Read())
                {
                    IDataReader reader = result.Reader;

                    yield return new WorldEditAction()
                    {
                        AccountID = reader.GetInt32(0),
                        Action = reader.GetString(1),
                        FullAction = reader.GetString(2),
                        Executed = reader.GetDateTime(3),
                        X = reader.GetInt32(4),
                        Y = reader.GetInt32(5),
                        X2 = reader.GetInt32(6),
                        Y2 = reader.GetInt32(7),
                        WorldID = reader.GetInt32(8)
                    };
                }
            }
        }
    }

    public struct WorldEditAction
    {
        public int AccountID;

        public string Action;
        public string FullAction;

        public DateTime Executed;

        public int X;
        public int Y;
        public int X2;
        public int Y2;

        public int WorldID;

        public override string ToString() =>
            $"Player [c/eb349e:{UserAccounts.GetUserAccountByID(AccountID)?.Name ?? AccountID.ToString()}] executed [c/FFB266:{Action}] ([c/FFCC99:/{FullAction}]) in [c/009900:{Executed.ToString("u")}].";
    }
}
