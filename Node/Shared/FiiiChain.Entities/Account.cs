using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class Account
    {
        public string Id { get; set; }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        public long Balance { get; set; }

        public bool IsDefault { get; set; }

        public bool WatchedOnly { get; set; }

        public long Timestamp { get; set; }

        public string Tag { get; set; }
    }
}
