namespace EthereumBalance.POCOs.Requests
{
    public interface IParityRequest
    {
        string method { get; set; }
        object @params { get; set; }
        object id { get; set; }
        string jsonrpc { get; set; }
    }
}