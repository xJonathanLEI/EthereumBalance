using EthereumBalance.Extensions;

namespace EthereumBalance.POCOs.Requests
{
    public class GetBalanceRequest : IParityRequest
    {
        public string method { get; set; }
        public object @params { get; set; }
        public object id { get; set; }
        public string jsonrpc { get; set; }

        public GetBalanceRequest(byte[] address, long blockHeight)
        {
            method = "eth_getBalance";
            @params = new object[] { address.ToHex(), blockHeight.ToBigEndianBytes().ToHex() };
            id = 1;
            jsonrpc = "2.0";
        }
    }
}