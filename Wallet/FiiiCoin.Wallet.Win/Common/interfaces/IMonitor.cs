namespace FiiiCoin.Wallet.Win.Common.interfaces
{
    public delegate void MonitorCallBackHandle<T>(T outParam);
    public interface IServiceMonitor<T> : IServiceMonitor
    {
        event MonitorCallBackHandle<T> MonitorCallBack;
    }

    public interface IServiceMonitor
    {
        void Start(int interval);
        void Stop();
    }

}
