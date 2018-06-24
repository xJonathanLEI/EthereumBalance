using EthereumBalance.Extensions;

namespace EthereumBalance.POCOs.Requests
{
    public class GetBlockByNumberRequest : IParityRequest
    {
        public string method { get; set; }
        public object @params { get; set; }
        public object id { get; set; }
        public string jsonrpc { get; set; }

        public GetBlockByNumberRequest(long blockHeight)
        {
            method = "eth_getBlockByNumber";
            @params = new object[] { blockHeight.ToBigEndianBytes().ToHex(), false };
            id = 1;
            jsonrpc = "2.0";
        }
    }
}