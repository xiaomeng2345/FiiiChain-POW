using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class CommandNames
    {
        public class P2P
        {
            public const string Ping = "Ping";
            public const string Pong = "Pong";
            public const string Version = "Version";
            public const string VerAck = "VerAck";
            public const string GetAddr = "GetAddr";
            public const string Addr = "Addr";
            public const string Heartbeat = "Heartbeat";
        }

        public class Transaction
        {
            public const string GetTxPool = "GetTxPool";
            public const string TxPool = "TxPool";
            public const string GetTx = "GetTx";
            public const string Tx = "Tx";
            public const string NewTx = "NewTx";
        }

        public class Block
        {
            public const string GetHeight = "GetHeight";
            public const string Height = "Height";
            public const string GetHeaders = "GetHeaders";
            public const string Headers = "Headers";
            public const string GetBlocks = "GetBlocks";
            public const string Blocks = "Blocks";
            public const string NewBlock = "NewBlock";
        }

        public class Other
        {
            public const string Reject = "Reject";
            public const string NotFound = "NotFound";
        }
    }
}
