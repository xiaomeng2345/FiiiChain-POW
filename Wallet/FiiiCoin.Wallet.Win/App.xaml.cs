using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Common;
using System;
using System.Windows;
using FiiiCoin.Wallet.Win.Biz.Monitor;
using FiiiCoin.Utility;

namespace FiiiCoin.Wallet.Win
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Action startupAction = () => { base.OnStartup(e); };
            SinlgeWindowStart(startupAction);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Singleton.Info(e.Exception.Message);
            ServiceMonitor.StopAll();
            var result = EngineService.Default.AppClosed();
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            ServiceMonitor.StopAll();
            var result = EngineService.Default.AppClosed();
            
        }

        void SinlgeWindowStart(Action startupAction)
        {
            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, "FiiiCoin.Wallet", out createNew))
            {
                if (createNew)
                {
                    LanguageService.Default.SetLanguage(AppSettingConfig.Default.AppConfig.LanguageType);
                    startupAction();
                    var shell = BootStrapService.Default.Shell.GetWindow();
                    if (shell != null)
                        shell.ShowDialog();
                    Application.Current.Exit += Current_Exit;
                    Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                }
                else
                {
                    MessageBox.Show("Application is already running...");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
