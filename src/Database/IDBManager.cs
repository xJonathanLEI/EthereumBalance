using System;

namespace EthereumBalance.Database
{
    public interface IDBManager : IDisposable
    {
        BlockDBManager blockMgr { get; }

        void InitializeTables();
    }
}