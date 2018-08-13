using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetTxSettingsOM
    {
        public long Confirmations { get; set; }
        public long FeePerKB { get; set; }
        public bool Encrypt { get; set; }
    }
}
