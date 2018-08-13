using FiiiCoin.Wallet.Win.Common.interfaces;
using System.Collections.Generic;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public sealed class ServiceMonitor
    {
        public static List<IServiceMonitor> Monitors { get; } = new List<IServiceMonitor>();

        public static void StopAll()
        {
            if (Monitors != null)
            {
                Monitors.ForEach(x => x.Stop());
            }
        }
    }
}
