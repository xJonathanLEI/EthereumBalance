using System;
using Microsoft.Data.Sqlite;
using EthereumBalance.Extensions;

namespace EthereumBalance.Database
{
    public abstract class GenericDBManager
    {
        protected SqliteConnection connection { get; set; }

        public GenericDBManager(SqliteConnection connection)
        {
            this.connection = connection;
        }

        protected SqliteDataReader ExecuteQuery(string command, object[] paramaters = null)
        {
            return ProcessCommand(command, paramaters).ExecuteReader();
        }

        protected int ExecuteNonQuery(string command, object[] paramaters = null)
        {
            return ProcessCommand(command, paramaters).ExecuteNonQuery();
        }

        protected SqliteDataReader ExecuteQuery(string command, SqliteTransaction transaction, object[] paramaters = null)
        {
            return ProcessCommand(command, paramaters, transaction).ExecuteReader();
        }

        protected int ExecuteNonQuery(string command, SqliteTransaction transaction, object[] paramaters = null)
        {
            return ProcessCommand(command, paramaters, transaction).ExecuteNonQuery();
        }

        private SqliteCommand ProcessCommand(string command, object[] paramaters)
        {
            SqliteCommand cmd = new SqliteCommand(command, connection);
            if (paramaters != null)
                for (int i = 0; i < paramaters.Length; i++)
                    if (paramaters[i] is DateTime)
                        cmd.Parameters.AddWithValue("@" + i.ToString(), ((DateTime)paramaters[i]).ToUnixTime());
                    else
                        cmd.Parameters.AddWithValue("@" + i.ToString(), paramaters[i]);
            return cmd;
        }

        private SqliteCommand ProcessCommand(string command, object[] paramaters, SqliteTransaction transaction)
        {
            SqliteCommand cmd = new SqliteCommand(command, connection, transaction);
            if (paramaters != null)
                for (int i = 0; i < paramaters.Length; i++)
                    if (paramaters[i] is DateTime)
                        cmd.Parameters.AddWithValue("@" + i.ToString(), ((DateTime)paramaters[i]).ToUnixTime());
                    else
                        cmd.Parameters.AddWithValue("@" + i.ToString(), paramaters[i]);
            return cmd;
        }
    }
}