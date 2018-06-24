namespace EthereumBalance.POCOs.Responses
{
    public class GenericParityResponse<T>
    {
        public object id { get; set; }
        public string jsonrpc { get; set; }
        public T result { get; set; }
    }
}