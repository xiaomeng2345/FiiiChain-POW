using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Models;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public class UpdateBlocksMonitor : ServiceMonitorBase<BlockSyncInfo>
    {
        private static UpdateBlocksMonitor _default;

        public static UpdateBlocksMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new UpdateBlocksMonitor();
                return _default;
            }
        }

        public BlockSyncInfo blockSyncInfo = new BlockSyncInfo();
        protected override BlockSyncInfo ExecTaskAndGetResult()
        {
            var result = NetWorkService.Default.GetBlockChainInfoSync(blockSyncInfo);
            if (result.ApiResponse.Error != null && result.ApiResponse.Error.Code != 0)
                return null;
            if (!result.Value.IsSyncStart())
                return null;
            return result.Value;
        }

        public void Reset()
        {
            blockSyncInfo = new BlockSyncInfo();
        }
    }
}
