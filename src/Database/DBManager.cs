using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using EthereumBalance.Constants;

namespace EthereumBalance.Database
{
    public class DBManager : IDBManager
    {
        public BlockDBManager blockMgr { get; }

        private SqliteConnection connection;

        public DBManager(string sqlitePath)
        {
            connection = new SqliteConnection($"Data Source={sqlitePath}");
            connection.Open();
            blockMgr = new BlockDBManager(connection);
        }

        public void InitializeTables()
        {
            string[] creations = new string[] {
                DBTblCreation.TABLE_BLOCKS_CREATION
            };
            foreach (string creation in creations)
                new SqliteCommand(creation, connection).ExecuteNonQuery();
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}