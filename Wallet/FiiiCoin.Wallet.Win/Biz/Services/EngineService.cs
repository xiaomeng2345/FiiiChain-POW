using FiiiCoin.Business;
using FiiiCoin.Models;
using FiiiCoin.Utility.Api;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Biz.Services
{
    public class EngineService : InstanceBase<EngineService>
    {
        public IResult AppClosed()
        {
            /* 当应用程序关闭的时候不能直接关闭，因为某些程序可能还在运行，如果立即结束，有可能造成数据丢失
             * 1、当应用程序关闭的时候需要即时判断BlockChainStatus的状态，如果是Stoped就可以结束了，如果是Stopping或者Running不能结束
             * 需要用try catch捕获异常来判断是否关闭
             */
            //应用程序关闭的时候先调用StopEngine的接口
            Result result = new Result();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Task stopEngineTask = new Task(async () =>
            {
                await BlockChainEngineApi.StopEngine();
            });
            stopEngineTask.Start();
            
            //即时判断BlockChainStatus
            try
            {
                Task task = new Task(async () =>
                {
                    ApiResponse response = await BlockChainEngineApi.GetBlockChainStatus();
                    if (!response.HasError)
                    {
                        bool flag = true;
                        while (flag)
                        {
                            BlockChainStatus blockChainStatus = response.GetResult<BlockChainStatus>();
                            if (blockChainStatus.RpcService == "Stopped")
                            {
                                flag = false;
                                autoResetEvent.Set();
                            }
                            Thread.Sleep(1000);
                        }
                    }
                });
                task.Start();
            }
            catch
            {
                //在这里捕获错误，然后关闭整个Application
                result.IsFail = true;
                autoResetEvent.Set();
            }
            autoResetEvent.WaitOne();
            result.IsFail = false;
            return result;
        }
    }
}
