using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Models.UiModels
{
    public class MoneyInfo : ViewModelBase
    {
        private double _money = 0d;
        public string _unit = "FIII";

        public double Money
        {
            get { return _money; }
            set { _money = value; RaisePropertyChanged("Money"); RaisePropertyChanged("MoneyUnit"); }
        }

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; RaisePropertyChanged("Unit"); }
        }

        public string MoneyUnit
        {
            get { return this.ToString(); }
        }

        public override string ToString()
        {
            return string.Format("{0}  {1}", Money.ToString("0.00000000"), Unit);
        }
    }
}
