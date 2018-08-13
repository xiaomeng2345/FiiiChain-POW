using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class ListLockUnspentOM
    {
        public string txid { get; set; }
        public int vout { get; set; }
    }

}
