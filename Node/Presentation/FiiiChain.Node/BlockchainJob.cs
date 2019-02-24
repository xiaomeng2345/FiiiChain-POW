// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;
using FiiiChain.Business;
using FiiiChain.Framework;

namespace FiiiChain.Node
{
    public class BlockchainJob : BaseJob
    {
        public static BlockchainJob Current = null;
        public BlockJob BlockService;
        public MinerJob MinerService;
        public TransactionJob TransactionService;
        public P2PJob P2PJob;
        public RpcJob RpcService;

        public BlockchainJob()
        {
            RpcService = new RpcJob();
            P2PJob = new P2PJob();
            BlockService = new BlockJob();
            MinerService = new MinerJob("John Liu Qiang");
            TransactionService = new TransactionJob();
        }
        public override JobStatus Status
        {
            get
            {
                if(P2PJob.Status == JobStatus.Running &&
                    RpcService.Status == JobStatus.Running &&
                    BlockService.Status == JobStatus.Running)
                {
                    return JobStatus.Running;
                }
                else if(P2PJob.Status == JobStatus.Stopped && 
                    RpcService.Status == JobStatus.Stopped &&
                    BlockService.Status == JobStatus.Stopped)
                {
                    return JobStatus.Stopped;
                }
                else
                {
                    return JobStatus.Stopping;
                }
            }
        }

        public override void Start()
        {
            P2PJob.Start();
            RpcService.Start();
            BlockService.Start();
            MinerService.Start();
            //TransactionService.Start();
        }

        public override void Stop()
        {
            P2PJob.Stop();
            RpcService.Stop();
            BlockService.Stop();
            MinerService.Stop();
            TransactionService.Stop();
        }

        public Dictionary<string, string> GetJobStatus()
        {
            var dict = new Dictionary<string, string>();

            dict.Add("ChainService", this.Status.ToString());
            dict.Add("P2pService", P2PJob.Status.ToString());
            dict.Add("BlockService", BlockService.Status.ToString());
            dict.Add("RpcService", RpcService.Status.ToString());
            dict.Add("ChainNetwork", GlobalParameters.IsTestnet ? "Testnet" : "Mainnet");
            dict.Add("Height", new BlockComponent().GetLatestHeight().ToString());

            return dict;
        }

        public static void Initialize()
        {
            BlockchainComponent blockChainComponent = new BlockchainComponent();
            AccountComponent accountComponent = new AccountComponent();
            UtxoComponent utxoComponent = new UtxoComponent();

            blockChainComponent.Initialize();
            var accounts = accountComponent.GetAllAccounts();

            if (accounts.Count == 0)
            {
                var account = accountComponent.GenerateNewAccount();
                accountComponent.SetDefaultAccount(account.Id);
            }

            BlockchainJob.Current = new BlockchainJob();
            utxoComponent.Initialize();
        }
    }
}
