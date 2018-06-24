using System.Collections.Generic;

namespace EthereumBalance.POCOs.Responses
{
    public class TokenBalance
    {
        public string Symbol { get; set; }
        public string Balance { get; set; }

        public TokenBalance(string symbol, string balance)
        {
            this.Symbol = symbol;
            this.Balance = balance;
        }
    }
}