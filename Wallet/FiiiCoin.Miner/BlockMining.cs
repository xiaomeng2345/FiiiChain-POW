// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using FiiiCoin.Business;
using FiiiCoin.DTO;
using FiiiCoin.Utility;
using FiiiCoin.Utility.Api;
using FiiiCoin.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiCoin.Miner
{
    public class BlockMining
    {
        public static async Task MiningAsync(string minerName, string walletAddress)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("new mining start");
                    bool isStop = false;
                    bool isHeightChanged = false;
                    ApiResponse response = await BlockChainEngineApi.GenerateNewBlock(minerName, walletAddress, 1);
                    if (!response.HasError)
                    {

                        BlockInfoOM block = response.GetResult<BlockInfoOM>();

                        List<byte> blockData = new List<byte>();

                        foreach (BlockTransactionsOM tx in block.Transactions)
                        {
                            blockData.AddRange(tx.Serialize());
                        }
                        string strDifficulty = string.Empty;

                        ApiResponse difficultyResponse = await BlockChainEngineApi.GetDifficulty();
                        if (!difficultyResponse.HasError)
                        {
                            BlockDifficultyOM blockDifficulty = difficultyResponse.GetResult<BlockDifficultyOM>();
                            strDifficulty = blockDifficulty.HashTarget;
                        }
                        else
                        {
                            Logger.Singleton.Error(difficultyResponse.Error.Code.ToString());
                            Logger.Singleton.Error(difficultyResponse.Error.Message);
                            Console.WriteLine(response.Error.Message);
                            return;
                        }
                        //var cts = new CancellationTokenSource();
                        //var ct = cts.Token;
                        //开启新线程
                        /*
                        Task task1 = Task.Run(async () => 
                        {  
                            try
                            {
                                Console.WriteLine("new thread validate block height");
                                int count = 0;
                                while (!isStop)
                                {
                                    ApiResponse tempResponse = await BlockChainEngineApi.GetBlockCount();
                                    if (!tempResponse.HasError)
                                    {
                                        count = 0;
                                        Logger.Singleton.Info($"current count is {count}, current datetime is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}");
                                        long height = tempResponse.GetResult<long>();
                                        Console.WriteLine($"current height of the chain is {height}");
                                        if (height >= block.Header.Height)
                                        {
                                            isStop = true;
                                            isHeightChanged = true;
                                            ct.ThrowIfCancellationRequested();
                                            cts.Cancel();
                                        }
                                    }
                                    else
                                    {
                                        count++;
                                        Logger.Singleton.Warn($"current count is {count}, current datetime is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}");
                                        if (count >= 5)
                                        {
                                            Logger.Singleton.Error(tempResponse.Error.Code.ToString());
                                            Logger.Singleton.Error(tempResponse.Error.Message);
                                            isStop = true;
                                            ct.ThrowIfCancellationRequested();
                                            cts.Cancel();
                                        }
                                    }
                                    
                                    Console.WriteLine("thread will sleep 5 seconds");
                                    Thread.Sleep(3000);
                                }
                            }
                            catch (Exception ex)
                            {
                                cts.Cancel();
                                Console.WriteLine($"something wrong with the application interface, {ex.Message}");
                                isStop = true;
                            }
                        }, ct);
                        */
                        Thread checkHeightThread = new Thread(async () =>
                        {
                            try
                            {
                                Console.WriteLine("new thread validate block height");
                                int count = 0;
                                while (!isStop)
                                {
                                    ApiResponse tempResponse = await BlockChainEngineApi.GetBlockCount();
                                    if (!tempResponse.HasError)
                                    {
                                        count = 0;
                                        //Logger.Singleton.Info($"current count is {count}, current datetime is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}");
                                        long height = tempResponse.GetResult<long>();
                                        Console.WriteLine($"current height of the chain is {height}");
                                        if (height >= block.Header.Height)
                                        {
                                            isStop = true;
                                            isHeightChanged = true;
                                            //ct.ThrowIfCancellationRequested();
                                            //cts.Cancel();
                                        }
                                    }
                                    else
                                    {
                                        count++;
                                        Logger.Singleton.Warn($"current count is {count}, current datetime is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}");
                                        if (count >= 3)
                                        {
                                            Logger.Singleton.Error(tempResponse.Error.Code.ToString());
                                            Logger.Singleton.Error(tempResponse.Error.Message);
                                            isStop = true;
                                            //ct.ThrowIfCancellationRequested();
                                            //cts.Cancel();
                                        }
                                    }

                                    Console.WriteLine("thread will sleep 5 seconds");
                                    Thread.Sleep(3000);
                                }
                            }
                            catch (Exception ex)
                            {
                                //cts.Cancel();
                                Console.WriteLine($"something wrong with the application interface, {ex.Message}");
                                isStop = true;
                            }
                        });
                        checkHeightThread.Priority = ThreadPriority.Highest;
                        checkHeightThread.Start();
                        Parallel.For(0L, Int64.MaxValue, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async (i, loopState) =>
                        {
                            if (isStop)
                            {
                                Console.WriteLine($"new thread has been stopped, stop main thread");
                                loopState.Stop();
                                return;
                            }
                            List<byte> newBuffer = new List<byte>(blockData.ToArray());
                            byte[] nonceBytes = BitConverter.GetBytes(i);
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(nonceBytes);
                            }
                            newBuffer.AddRange(nonceBytes);
                            string result = Base16.Encode(
                                    HashHelper.Hash(
                                        newBuffer.ToArray()
                                    ));
                            Console.WriteLine($"current nonce is {i}, current datetime is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}");

                            if (BlockInfoOM.Verify(strDifficulty, result))
                            {
                                loopState.Stop();

                                //区块头的时间戳
                                block.Header.Timestamp = TimeHelper.EpochTime;
                                //区块头的随机数
                                block.Header.Nonce = i;
                                //区块头的hash
                                block.Header.Hash = block.Header.GetHash();
                                //提交区块
                                Console.WriteLine($"verify success. nonce is {i}, block hash is {block.Header.Hash}");
                                ApiResponse submitResponse = await BlockChainEngineApi.SubmitBlock(Base16.Encode(block.Serialize()));
                                if (!submitResponse.HasError)
                                {
                                    //停止循环
                                    //break;
                                    Logger.Singleton.Debug("A New Block " + block.Header.Height + "has been created, the correct nonce is " + i);
                                }
                                else
                                {
                                    Console.WriteLine(submitResponse.Error.Message);
                                    Console.WriteLine(submitResponse.Error.Code.ToString());
                                    if (submitResponse.Error.Code == 2060001)
                                    {
                                        isStop = true;
                                        isHeightChanged = true;
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                    }
                                }
                            }
                        });
                        if (isStop)
                        {
                            if (isHeightChanged)
                            {
                                Console.WriteLine("block height changed, new loop will start");
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("something wrong with the application interface, system exit");
                                Environment.Exit(0);
                                return;
                            }
                        }
                    }
                    else
                    {
                        Logger.Singleton.Error(response.Error.Code.ToString());
                        Logger.Singleton.Error(response.Error.Message);
                        Console.WriteLine(response.Error.Message);
                        Environment.Exit(0);
                        return;
                    }
                }
                catch(Exception ex)
                {
                    Logger.Singleton.Error(ex.ToString());
                    Logger.Singleton.Error(ex.Message);
                    Environment.Exit(0);
                    return;
                }
            }
        }
    }
}
