namespace EthereumBalance.Constants
{
    public static class DBConst
    {
        public const string TABLE_BLOCKS = "ethbal_blocks";
    }

    public static class DBTblCreation
    {
        public static string TABLE_BLOCKS_CREATION =
        $@"CREATE TABLE IF NOT EXISTS {DBConst.TABLE_BLOCKS} (
            id INTEGER PRIMARY KEY,
            height INTEGER NOT NULL UNIQUE,
            timestamp INTEGER NOT NULL
        )";
    }
}