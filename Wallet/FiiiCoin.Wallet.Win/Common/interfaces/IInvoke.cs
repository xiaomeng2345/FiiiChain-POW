using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Common.interfaces
{
    public interface IInvoke
    {
        string GetInvokeName();

        void Invoke<T>(T obj);
    }
}
