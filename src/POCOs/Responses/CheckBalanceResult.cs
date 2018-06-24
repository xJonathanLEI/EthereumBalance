using System.Collections.Generic;

namespace EthereumBalance.POCOs.Responses
{
    public class CheckBalanceResult
    {
        public long Block { get; set; }
        public string ETH { get; set; }
        public Dictionary<string, string> Tokens { get; set; }

        public CheckBalanceResult()
        {
            this.Tokens = new Dictionary<string, string>();
        }
    }
}