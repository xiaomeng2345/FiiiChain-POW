using FiiiCoin.Wallet.Win.Biz.Services;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public class NetInfoMonitor : ServiceMonitorBase<long?>
    {
        private static NetInfoMonitor _default;

        public static NetInfoMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new NetInfoMonitor();
                return _default;
            }
        }

        protected override long? ExecTaskAndGetResult()
        {
            var result = NetWorkService.Default.GetConnectionCount();
            if (result.IsFail)
                return null;
            return result.Value;
        }
    }
}
