using EthereumBalance.Extensions;

namespace EthereumBalance.POCOs.Requests
{
    public class GetStorageAtRequest : IParityRequest
    {
        public string method { get; set; }
        public object @params { get; set; }
        public object id { get; set; }
        public string jsonrpc { get; set; }

        public GetStorageAtRequest(byte[] contract, string position, long blockHeight)
        {
            method = "eth_getStorageAt";
            @params = new object[] { contract.ToHex(), position, blockHeight.ToBigEndianBytes().ToHex() };
            id = 1;
            jsonrpc = "2.0";
        }
    }
}