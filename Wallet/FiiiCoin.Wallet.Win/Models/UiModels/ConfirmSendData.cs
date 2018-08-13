using FiiiCoin.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Models.UiModels
{
    public class ConfirmSendData : NotifyBase
    {
        private double fee;

        public double Fee
        {
            get { return fee; }
            set { fee = value; RaisePropertyChanged("Fee"); }
        }

        private double amount;

        public double Amount
        {
            get { return amount; }
            set { amount = value; RaisePropertyChanged("Amount");  }
        }

        private string toAddress;

        public string ToAddress
        {
            get { return toAddress; }
            set { toAddress = value; RaisePropertyChanged("ToAddress"); }
        }

        private double arrivalAmount;

        public double ArrivalAmount
        {
            get { return arrivalAmount; }
            set { arrivalAmount = value; RaisePropertyChanged("ArrivalAmount"); RaisePropertyChanged("ArrivalAmount"); }
        }
    }
}
