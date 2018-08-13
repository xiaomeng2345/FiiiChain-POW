using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Common
{
    public abstract class SerializerBase<T> : InstanceBase<T> where T : class
    {
        public string ConfigFilePath = null;

        protected abstract T1 LoadConfig<T1>() where T1 :class;
    }
}
