namespace EthereumBalance.Configs
{
    public class ConfigObject
    {
        public string hostUrl { get; set; }
        public string fastNodeUrl { get; set; }
        public string archiveNodeUrl { get; set; }
        public int? avgBlockInterval { get; set; }
        public string sqlitePath { get; set; }
    }
}