using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Common
{
    public abstract class InstanceBase<T> where T : class
    {
        private static T instance = null;
        public static T Default
        {
            get
            {
                if (instance == null)
                {
                    try
                    {
                        instance = Activator.CreateInstance<T>();
                    }
                    catch
                    {
                        throw new Exception("InstanceBase Create a Default Value Fail");
                    }
                }
                return instance;
            }
            protected set
            {
                instance = value;
            }
        }
    }
}
