using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetAddedNodeInfoOM
    {
        public string address { get; set; }
        public string connected { get; set; }
        public long connectedTime { get; set; }
    }
}
