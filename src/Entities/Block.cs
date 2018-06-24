namespace EthereumBalance.Entities
{
    public class Block
    {
        public long Height { get; set; }
        public long Timestamp { get; set; }

        public Block(long height, long timestamp)
        {
            Height = height;
            Timestamp = timestamp;
        }
    }
}