using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using EthereumBalance.Entities;
using EthereumBalance.Constants;

namespace EthereumBalance.Database
{
    public class BlockDBManager : GenericDBManager
    {
        public BlockDBManager(SqliteConnection connection) : base(connection) { }

        public List<Block> ListBlocks()
        {
            var blocks = new List<Block>();
            using (var reader = ExecuteQuery($"SELECT (height, timestamp) FROM {DBConst.TABLE_BLOCKS}"))
                while (reader.Read())
                    blocks.Add(new Block(reader.GetInt64(0), reader.GetInt64(1)));
            return blocks;
        }

        public long InsertBlock(Block newBlock)
        {
            int rows = ExecuteNonQuery($"INSERT INTO {DBConst.TABLE_BLOCKS} (height, timestamp) VALUES (@0, @1)", new object[] { newBlock.Height, newBlock.Timestamp });
            if (rows == 0) return -1;
            using (var reader = ExecuteQuery($"SELECT LAST_INSERT_ROWID()"))
            {
                reader.Read();
                return reader.GetInt64(0);
            }
        }
    }
}