using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class P2PState
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public P2PCommand Command { get; set; }
    }
}
