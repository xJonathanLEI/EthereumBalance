using System;
using System.Text;
using System.Numerics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EthereumBalance.POCOs.Requests;
using EthereumBalance.POCOs.Responses;
using EthereumBalance.Caches;
using EthereumBalance.Configs;
using EthereumBalance.Entities;
using EthereumBalance.Database;
using EthereumBalance.Extensions;

namespace EthereumBalance.Services
{
    public class BalanceService
    {
        private Cache caches;
        private ConfigObject configs;
        private IServiceProvider isp;

        public BalanceService(IServiceProvider isp)
        {
            this.isp = isp;
            this.caches = isp.GetRequiredService<Cache>();
            this.configs = isp.GetRequiredService<ConfigObject>();
        }

        public async Task<CheckBalanceResult> CheckBalance(byte[] address, long timestamp)
        {
            var result = new CheckBalanceResult();

            Block targetBlock = await GetBlockAtTime(timestamp);
            result.Block = targetBlock.Height;

            Console.WriteLine($"Checking balances of {address.ToHex()} at {targetBlock.Height}");

            // Checks ETH balance
            BigInteger ethBalance;
            try
            {
                using (var hc = new HttpClient())
                {
                    var hrm = new HttpRequestMessage(HttpMethod.Post, configs.archiveNodeUrl);
                    hrm.Content = new StringContent(JsonConvert.SerializeObject(new GetBalanceRequest(address, targetBlock.Height)), Encoding.UTF8, "application/json");
                    var response = await hc.SendAsync(hrm);
                    string content = await response.Content.ReadAsStringAsync();
                    var resObj = JsonConvert.DeserializeObject<GenericParityResponse<string>>(content).result;
                    ethBalance = resObj.ToBigInteger();
                    result.ETH = ethBalance.ToDecString(18, 18);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error trying to check ETH balance: {ex.Message}");
                Console.WriteLine($"Error Trace: {ex}");
                return null;
            }

            return result;
        }

        public async Task<Block> GetBlockAtTime(long timestamp)
        {
            // Genesis block
            Block genesisBlock = await RequireBlock(1);

            // Nothing exists before genesis
            if (timestamp < genesisBlock.Timestamp)
                return null;

            while (true)
            {
                // Checks the closest timestamp
                long minLeftDiff = -1;
                Block minLeftDiffBlock = null;
                long minRightDiff = -1;
                Block minRightDiffBlock = null;
                caches.blocksLock.EnterReadLock();
                try
                {
                    foreach (var blockPair in caches.blocks)
                    {
                        var currentBlock = blockPair.Value;
                        if (currentBlock.Timestamp == timestamp)
                            // Bingo! Got it
                            return currentBlock;
                        else if (currentBlock.Timestamp > timestamp)
                        {
                            // On the right
                            long currentDiff = currentBlock.Timestamp - timestamp;
                            if (minRightDiff == -1 || currentDiff < minRightDiff)
                            {
                                minRightDiff = currentDiff;
                                minRightDiffBlock = currentBlock;
                            }
                        }
                        else
                        {
                            // On the left
                            long currentDiff = timestamp - currentBlock.Timestamp;
                            if (minLeftDiff == -1 || currentDiff < minLeftDiff)
                            {
                                minLeftDiff = currentDiff;
                                minLeftDiffBlock = currentBlock;
                            }
                        }
                    }
                }
                finally
                {
                    if (caches.blocksLock.IsReadLockHeld)
                        caches.blocksLock.ExitReadLock();
                }

                // Get the next block
                if (BlockInCache(minLeftDiffBlock.Height + 1, out Block leftNext) && leftNext.Timestamp > timestamp)
                    return minLeftDiffBlock;

                // Nothing on the right
                if (minRightDiffBlock == null)
                {
                    while (true)
                    {
                        // Estimate the target block
                        long estTargetHeight = minLeftDiffBlock.Height + minLeftDiff / configs.avgBlockInterval.Value;
                        if (estTargetHeight == minLeftDiffBlock.Height)
                            estTargetHeight++;

                        // Retrieves the target block
                        Block estTargetBlock = await RequireBlock(estTargetHeight);

                        // Block does not exist (too far!)
                        if (estTargetBlock == null)
                        {
                            // Adjusts block interval
                            configs.avgBlockInterval = (int)(configs.avgBlockInterval * 1.1);
                            Console.WriteLine($"Block interval auto adjusted to {configs.avgBlockInterval.Value}");
                        }
                        else break;
                    }
                }
                else
                {
                    // Something on the left (must be true with genesis) and something on the right
                    // The two blocks are NOT consecutive (or it'd have been returned)
                    long estTargetHeight = minLeftDiffBlock.Height +
                                           (long)(((double)minLeftDiff / (double)(minRightDiffBlock.Timestamp - minLeftDiffBlock.Timestamp)) *
                                           (minRightDiffBlock.Height - minLeftDiffBlock.Height));
                    if (estTargetHeight == minLeftDiffBlock.Height)
                        estTargetHeight++;
                    else if (estTargetHeight == minRightDiffBlock.Height)
                        estTargetHeight--;

                    // Retrieves the target block
                    Block estTargetBlock = await RequireBlock(estTargetHeight);
                }
            }
        }

        private async Task<Block> RequireBlock(long blockHeight)
        {
            // Retrieves from cache directly if presents
            if (BlockInCache(blockHeight, out var block))
                return block;

            // Retrieves from network
            var (success, downloadedBlock) = await CacheBlock(blockHeight);
            if (!success)
                return null;

            return downloadedBlock;
        }

        private bool BlockInCache(long blockHeight, out Block block)
        {
            try
            {
                caches.blocksLock.EnterReadLock();
                if (caches.blocks.ContainsKey(blockHeight))
                {
                    block = caches.blocks[blockHeight];
                    return true;
                }
                else
                {
                    block = null;
                    return false;
                }
            }
            finally
            {
                if (caches.blocksLock.IsReadLockHeld)
                    caches.blocksLock.ExitReadLock();
            }
        }

        private async Task<(bool, Block)> CacheBlock(long blockHeight)
        {
            long blockTimestamp;

            // Checks timestamp from parity node
            try
            {
                using (var hc = new HttpClient())
                {
                    Console.WriteLine($"Retrieving block {blockHeight} from {configs.fastNodeUrl}");
                    var hrm = new HttpRequestMessage(HttpMethod.Post, configs.fastNodeUrl);
                    hrm.Content = new StringContent(JsonConvert.SerializeObject(new GetBlockByNumberRequest(blockHeight)), Encoding.UTF8, "application/json");
                    var response = await hc.SendAsync(hrm);
                    string content = await response.Content.ReadAsStringAsync();
                    var resObj = JsonConvert.DeserializeObject<GenericParityResponse<GetBlockByNumberResult>>(content).result;
                    blockTimestamp = resObj.timestamp;
                    if (blockTimestamp <= 0)
                        throw new Exception($"Invalid response from parity node: {content}");
                    Console.WriteLine($"Block timestamp of {blockHeight}: {blockTimestamp}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error trying to cache block {blockHeight}: {ex.Message}");
                Console.WriteLine($"Error Trace: {ex}");
                return (false, null);
            }

            Block newBlock = new Block(blockHeight, blockTimestamp);

            // Writes into database
            using (var dbMgr = isp.GetRequiredService<IDBManager>())
                if (dbMgr.blockMgr.InsertBlock(newBlock) == -1)
                    return (false, null);

            // Adds to cache
            caches.blocksLock.EnterWriteLock();
            try
            {
                caches.blocks.Add(blockHeight, newBlock);
            }
            finally
            {
                if (caches.blocksLock.IsWriteLockHeld)
                    caches.blocksLock.ExitWriteLock();
            }

            return (true, newBlock);
        }
    }
}