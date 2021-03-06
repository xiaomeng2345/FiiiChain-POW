﻿using FiiiCoin.Wallet.Win.Biz.Monitor;
using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FiiiCoin.Wallet.Win.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            if (IsInDesignMode)
            {
                #region menu菜单
                AppSetting config = new AppSetting();
                var fileMenu = new MenuInfo("文件");
                fileMenu.MenuItems.Add(new MenuInfo("打开URI"));
                fileMenu.MenuItems.Add(new MenuInfo("备份钱包"));
                fileMenu.MenuItems.Add(new MenuInfo("消息签名"));
                fileMenu.MenuItems.Add(new MenuInfo("验证消息"));
                fileMenu.MenuItems.Add(new MenuSeparator());
                fileMenu.MenuItems.Add(new MenuInfo("正在发送地址"));
                fileMenu.MenuItems.Add(new MenuInfo("正在接受地址"));
                fileMenu.MenuItems.Add(new MenuSeparator());
                fileMenu.MenuItems.Add(new MenuInfo("退出"));
                MenuItems.Add(fileMenu);

                var settingMenu = new MenuInfo("设置");
                settingMenu.MenuItems.Add(new MenuInfo("加密钱包"));
                settingMenu.MenuItems.Add(new MenuInfo("更改密码"));
                settingMenu.MenuItems.Add(new MenuSeparator());
                settingMenu.MenuItems.Add(new MenuInfo("选项"));
                MenuItems.Add(settingMenu);

                var helpMenu = new MenuInfo("帮助");
                helpMenu.MenuItems.Add(new MenuInfo("调试窗口"));
                helpMenu.MenuItems.Add(new MenuInfo("命令行选项"));
                helpMenu.MenuItems.Add(new MenuSeparator());
                helpMenu.MenuItems.Add(new MenuInfo("关于Bitcoin Core"));
                MenuItems.Add(helpMenu);
                #endregion
            }
            else
            {
                Init();
            }
        }

        void Init()
        {
            if (AppSettingConfig.Default.AppConfig == null)
                return;

            if (AppSettingConfig.Default.AppConfig.MenuItems != null)
            {
                AppSettingConfig.Default.AppConfig.MenuItems.ForEach(x => MenuItems.Add(x));
            }

            WindowCommand = new RelayCommand<string>(OnWindowCommand);
            MenuCommand = new RelayCommand<MenuInfo>(OnMenuClick);

            Messenger.Default.Register<bool>(this, MessageTopic.ChangedPopupViewState, OnChangePopupView);
            Messenger.Default.Register<string>(this, MessageTopic.ShowMessageAutoClose, OnShowMessageView);
            Messenger.Default.Register<string>(this, MessageTopic.UpdateMainView, OnUpdateMainView);
        }

        private const string _startPageName = Pages.InitPage;
        private const string _popupPageName = Pages.PopupShell;
        private Page _contentView;
        private Page _popupView;
        private string _message;
        private ObservableCollection<MenuInfo> _menuItems;
        private bool _isShowPopupView = false;
        private bool _isShowMessage = false;
        private bool _isIniting = true;

        public bool IsIniting
        {
            get { return _isIniting; }
            set
            {
                _isIniting = value;
                RaisePropertyChanged("IsIniting");
            }
        }

        public ObservableCollection<MenuInfo> MenuItems
        {
            get
            {
                if (_menuItems == null)
                    _menuItems = new ObservableCollection<MenuInfo>();
                return _menuItems;
            }
            set
            {
                _menuItems = value;
                RaisePropertyChanged("MenuItems");
            }
        }

        public Page ContentView
        {
            get { return _contentView; }
            set { _contentView = value; RaisePropertyChanged("ContentView"); }
        }

        public Page PopupView
        {
            get { return _popupView; }
            set { _popupView = value; RaisePropertyChanged("PopupView"); }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged("Message"); }
        }

        public bool IsShowPopupView
        {
            get { return _isShowPopupView; }
            set
            {
                _isShowPopupView = value;
                RaisePropertyChanged("IsShowPopupView");
            }
        }

        public bool IsShowMessage
        {
            get { return _isShowMessage; }
            set
            {
                _isShowMessage = value;
                RaisePropertyChanged("IsShowMessage");
            }
        }


        public ICommand WindowCommand { get; private set; }
        public ICommand MenuCommand { get; private set; }


        void OnWindowCommand(string msg)
        {
            WindowCommandMode mode;
            if (!Enum.TryParse(msg.ToUpper(), out mode))
                return;

            var window = BootStrapService.Default.Shell.GetWindow();
            if (window == null) return;

            switch (mode)
            {
                case WindowCommandMode.MIN: window.WindowState = WindowState.Minimized; break;
                case WindowCommandMode.MAX:
                    var windowSatate = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    window.WindowState = windowSatate;
                    break;
                case WindowCommandMode.CLOSE:
                    window.Close();
                    break;
                case WindowCommandMode.ONLOADED:
                    ContentView = BootStrapService.Default.GetPage(_startPageName);
                    PopupView = BootStrapService.Default.GetPage(_popupPageName);
                    break;
                default:
                    break;
            }
        }

        void OnMenuClick(MenuInfo menuInfo)
        {
            IsIniting = false;
            if (menuInfo.ViewName.StartsWith("Invoke:"))
            {
                var invokeName = menuInfo.ViewName.Replace("Invoke:", "").Trim();
                BootStrapService.Default.Invoke(invokeName);
                return;
            }
            else if (menuInfo.ViewName.Contains(":"))
            {
                var @params = menuInfo.ViewName.Split(':');
                var pageName = @params[0];
                Messenger.Default.Send(@params[1],pageName);
                Messenger.Default.Send(pageName, MessageTopic.UpdatePopupView);
            }
            else
            {
                Messenger.Default.Send(menuInfo.ViewName, MessageTopic.UpdatePopupView);
            }
            IsShowPopupView = true;
        }

        void OnChangePopupView(bool isOpen)
        {
            IsShowPopupView = isOpen;
        }

        void OnShowMessageView(string msg)
        {
            Message = msg;
            IsShowMessage = true;
            Task task = new Task(CloseMessageShow);
            task.Start();
        }

        void OnUpdateMainView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
                return;
            ContentView = BootStrapService.Default.GetPage(viewName);
            IsIniting = false;
        }

        void CloseMessageShow()
        {
            Thread.Sleep(1000);

            Application.Current.Dispatcher.Invoke(() => {
                IsShowMessage = false;
            });
        }
    }

    enum WindowCommandMode
    {
        MIN,
        MAX,
        CLOSE,
        ONLOADED
    }

}
