using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetDifficultyOM
    {
        public long height { get; set; }
        public string hashTarget { get; set; }
    }
}
