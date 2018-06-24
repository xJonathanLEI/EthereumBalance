using System.Threading;
using System.Collections.Generic;
using EthereumBalance.Entities;

namespace EthereumBalance.Caches
{
    public class Cache
    {
        public SortedList<long, Block> blocks { get; set; }
        public ReaderWriterLockSlim blocksLock { get; set; }

        public Cache()
        {
            this.blocks = new SortedList<long, Block>();
            this.blocksLock = new ReaderWriterLockSlim();
        }
    }
}