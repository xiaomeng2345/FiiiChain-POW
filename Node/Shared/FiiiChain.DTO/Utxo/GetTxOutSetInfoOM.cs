using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetTxOutSetInfoOM
    {
        public long height { get; set; }
        public string bestblock { get; set; }
        public long transactions { get; set; }
        public long txouts { get; set; }
        //public int bytes_serialized { get; set; }
        //public string hash_serialized { get; set; }
        public long total_amount { get; set; }
    }
}
