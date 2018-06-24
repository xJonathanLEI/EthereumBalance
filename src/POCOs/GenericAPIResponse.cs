using EthereumBalance.Constants;

namespace EthereumBalance.POCOs
{
    public class GenericAPIResponse<T>
    {
        public T data;
        public ResponseCode code;

        public GenericAPIResponse(T data, ResponseCode code = ResponseCode.Success)
        {
            this.data = data;
            this.code = code;
        }
    }
}