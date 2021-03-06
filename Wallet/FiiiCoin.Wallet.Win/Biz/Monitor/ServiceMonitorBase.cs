﻿using FiiiCoin.Wallet.Win.Common.interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public abstract class ServiceMonitorBase<T> : IServiceMonitor<T>
    {
        public event MonitorCallBackHandle<T> MonitorCallBack;

        bool isStop = true;
        int sleepInterval;
        public void Start(int interval)
        {
            sleepInterval = interval;
            if (!isStop)
            {
                return;
            }
            if (!ServiceMonitor.Monitors.Contains(this))
                ServiceMonitor.Monitors.Add(this);
            isStop = false;
            Task task = new Task(() =>
            {
                CyclicTask();
            });
            task.Start();
        }

        public void Stop()
        {
            isStop = true;
        }

        private void CyclicTask()
        {
            while (!isStop)
            {
                try
                {
                    if (MonitorCallBack == null)
                        continue;

                    var result = ExecTaskAndGetResult();
                    if (result != null)
                        MonitorCallBack.Invoke(result);

                    Thread.Sleep(sleepInterval);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected abstract T ExecTaskAndGetResult();

        protected void CallBack(T innerParmas)
        {
            if (MonitorCallBack != null)
            {
                MonitorCallBack.Invoke(innerParmas);
            }
        }
    }
}
