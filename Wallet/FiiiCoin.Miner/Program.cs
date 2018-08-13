// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using FiiiCoin.Business;
using FiiiCoin.Models;
using FiiiCoin.Utility;
using FiiiCoin.Utility.Api;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiiiCoin.Miner
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(args);
            //string userCommand = "";
            //while (userCommand.ToLower() != "exit")
            //{
            //    userCommand = Console.ReadLine();
            //    string[] input = userCommand.Split(" ");
            //    Run(input);
            //}
            //Console.ReadKey();
        }

        static void Run(string[] args)
        {
            var app = new CommandLineApplication(false);
            app.HelpOption("-?|-h|--help");
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return;
            });
            app.Command("mining", command =>
            {
                command.Description = "begin mining";
                command.HelpOption("-?|-h|--help");
                CommandArgument nameArgument = command.Argument("[minerName]", "minerName");
                CommandArgument addressArgument = command.Argument("[walletAddress]", "walletAddress");
                command.OnExecute(async () =>
                {
                    if (nameArgument != null && !string.IsNullOrEmpty(nameArgument.Value) && addressArgument != null && !string.IsNullOrEmpty(addressArgument.Value))
                    {
                        string name = nameArgument.Value;
                        string address = addressArgument.Value;
                        Program program = new Program();
                        BlockChainStatus chainStatus = await program.GetChainStatus();
                        if (chainStatus == null)
                        {
                            app.Out.WriteLine("there is something wrong with the api, you should check the fiiichain");
                            return;
                        }
                        //验证本地区块高度和网络区块高度
                        ApiResponse response = await NetworkApi.GetBlockChainInfo();
                        if (!response.HasError)
                        {
                            BlockChainInfo info = response.GetResult<BlockChainInfo>();
                            if (info.IsRunning)
                            {
                                if (info.RemoteLatestBlockHeight <= info.LocalLastBlockHeight)
                                {
                                    command.Out.WriteLine($"current network is {chainStatus.ChainNetwork}");
                                    //validate wallet address
                                    if (AddressTools.AddressVerfy(chainStatus.ChainNetwork, address))
                                    {
                                        command.Out.WriteLine($"address verify success. prepare to mine");
                                        await BlockMining.MiningAsync(name, address);
                                    }
                                    else
                                    {
                                        command.Out.WriteLine($"address verify fail. address: {address} is invalid");
                                        return;
                                    }
                                }
                                else
                                {
                                    command.Out.WriteLine("Block Chain is in sync, please try it later");
                                    return;
                                }
                            }
                            else
                            {
                                command.Out.WriteLine("Block Chain has stopped");
                                return;
                            }
                        }
                        else
                        {
                            command.Out.WriteLine(response.Error.Message);
                            return;
                        }
                        
                    }
                    else
                    {
                        command.ShowHelp();
                        return;
                    }
                });
            });
            app.Command("height", command =>
            {
                command.Description = "view current block height";
                command.OnExecute(async () =>
                {
                    Program program = new Program();
                    BlockChainStatus chainStatus = await program.GetChainStatus();
                    if (chainStatus == null)
                    {
                        app.Out.WriteLine("there is something wrong with the api, you should check the fiiichain");
                        return;
                    }
                    ApiResponse response = await BlockChainEngineApi.GetBlockCount();
                    if (!response.HasError)
                    {
                        long result = response.GetResult<long>();
                        command.Out.WriteLine($"current block height is {result}");
                    }
                    else
                    {
                        command.Out.WriteLine($"{response.Error.Message}");
                    }
                });
            });
            app.Command("balance", command =>
            {
                command.Description = "view wallet balance";
                command.OnExecute(async () =>
                {
                    Program program = new Program();
                    BlockChainStatus chainStatus = await program.GetChainStatus();
                    if (chainStatus == null)
                    {
                        app.Out.WriteLine("there is something wrong with the api, you should check the fiiichain");
                        return;
                    }
                    ApiResponse response = await UtxoApi.GetTxOutSetInfo();
                    if (!response.HasError)
                    {
                        TxOutSet set = response.GetResult<TxOutSet>();
                        command.Out.WriteLine($"wallet balance is ：{set.Total_amount}");
                    }

                });
            });
            app.Command("transaction", command =>
            {
                command.Description = "view recent transaction record（default 5 content）";
                CommandArgument recordArgument = command.Argument("[count]", "record content");
                command.OnExecute(async () =>
                {
                    if (recordArgument != null && !string.IsNullOrEmpty(recordArgument.Value))
                    {
                        if (int.TryParse(recordArgument.Value, out int count))
                        {
                            Program program = new Program();
                            BlockChainStatus chainStatus = await program.GetChainStatus();
                            if (chainStatus == null)
                            {
                                app.Out.WriteLine("there is something wrong with the api, you should check the fiiichain");
                                return;
                            }
                            ApiResponse response = await TransactionApi.ListTransactions("*", count);
                            if (!response.HasError)
                            {
                                List<Payment> result = response.GetResult<List<Payment>>();
                                if (result != null && result.Count > 0)
                                {
                                    command.Out.WriteLine("recent transaction record blow：");
                                    foreach (var item in result)
                                    {
                                        //time（需要转换为DataTime）， comment， amount
                                        string time = new DateTime(1970, 1, 1).AddMilliseconds(item.Time).ToString("yyyy-MM-dd HH:mm:ss");
                                        command.Out.WriteLine($"Time:{time}; Comment:{item.Comment}; Amount:{item.Amount}");
                                    }
                                }
                                else
                                {
                                    command.Out.WriteLine("no recent transaction record.");
                                }
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                        }
                    }
                    else
                    {
                        Program program = new Program();
                        BlockChainStatus chainStatus = await program.GetChainStatus();
                        if (chainStatus == null)
                        {
                            app.Out.WriteLine("there is something wrong with the api, you should check the fiiichain");
                            return;
                        }
                        ApiResponse response = await TransactionApi.ListTransactions();
                        if (!response.HasError)
                        {
                            List<Payment> result = response.GetResult<List<Payment>>();
                            if (result != null && result.Count > 0)
                            {
                                command.Out.WriteLine("recent transaction record blow：");
                                foreach (var item in result)
                                {
                                //time（需要转换为DataTime）， comment， amount
                                string time = new DateTime(1970, 1, 1).AddMilliseconds(item.Time).ToString("yyyy-MM-dd HH:mm:ss");
                                    command.Out.WriteLine($"Time:{time}; Comment:{item.Comment}; Amount:{item.Amount}");
                                }
                            }
                            else
                            {
                                command.Out.WriteLine("no recent transaction record.");
                            }
                        }
                    }
                });
            });
            /*
            if (args.Length > 1 && args[0].ToLower() == "fiiipay" && IsContainsCommand(args[1]))
            {
                List<string> list = new List<string>(args);
                list.RemoveAt(0);
                app.Execute(list.ToArray());
            }
            */
            if (args != null && args.Length > 0 && IsContainsCommand(args[0]))
            {
                app.Execute(args);
            }
            else
            {
                app.Execute(new[] { "-?" });
            }
        }

        public static bool IsContainsCommand(string key)
        {
            string command = "mining|height|balance|transaction";
            string[] commandArray = command.Split('|');
            foreach (string item in commandArray)
            {
                if (item == key.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<BlockChainStatus> GetChainStatus()
        {
            ApiResponse api = await BlockChainEngineApi.GetBlockChainStatus();
            if (!api.HasError)
            {
                return api.GetResult<BlockChainStatus>();
            }
            else
            {
                return null;
            }
        }
    }
}
