using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.Entities;
using FiiiChain.Framework;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FiiiChain.Business
{
    public class BlockchainComponent
    {
        public void Initialize()
        {
            DBManager.Initialization();
        }
    }
}
