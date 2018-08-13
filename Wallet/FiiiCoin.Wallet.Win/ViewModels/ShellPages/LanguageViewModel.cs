using FiiiCoin.Wallet.Win.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class LanguageViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.LanguagePage;
        }

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        void OnSelectionChanged()
        {
            var langaugeType = (LanguageType)SelectedIndex;
            LanguageService.Default.SetLanguage(langaugeType);
        }

        public override void OnOkClick()
        {
            var langaugeType = (LanguageType)SelectedIndex;
            if (LanguageService.Default.LanguageType != langaugeType)
            {
                OnSelectionChanged();
                AppSettingConfig.Default.SwitchLanguage();
                AppSettingConfig.Default.SaveLanguageType(langaugeType);
            }
            base.OnOkClick();
        }

        protected override void OnLoaded()
        {
            SelectedIndex = (int)LanguageService.Default.LanguageType;
            base.OnLoaded();
        }
    }
}