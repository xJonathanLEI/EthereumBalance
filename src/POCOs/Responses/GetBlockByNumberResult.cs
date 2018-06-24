using Newtonsoft.Json;
using EthereumBalance.Converters;

namespace EthereumBalance.POCOs.Responses
{
    public class GetBlockByNumberResult
    {
        [JsonConverter(typeof(Int64HexConverter))]
        public long number;
        [JsonConverter(typeof(Int64HexConverter))]
        public long timestamp;
    }
}