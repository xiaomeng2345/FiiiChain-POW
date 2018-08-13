using FiiiCoin.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FiiiCoin.Wallet.Win.Models
{
    [XmlRoot]
    public class TimeGoalItem : VmBase
    {
        private string _value;

        [XmlIgnore]
        public string Value { get { return _value; } set { _value = value; RaisePropertyChanged("Value"); } }

        [XmlAttribute("Value")]
        public string ValueKey { get; set; }

        [XmlAttribute("Key")]
        public double Key { get; set; }
    }
}
