using System.Collections.Generic;

namespace EthereumBalance.POCOs.Responses
{
    public class CheckBalanceResult
    {
        public long Block { get; set; }
        public string ETH { get; set; }
        public List<TokenBalance> Tokens { get; set; }

        public CheckBalanceResult()
        {
            this.Tokens = new List<TokenBalance>();
        }
    }
}