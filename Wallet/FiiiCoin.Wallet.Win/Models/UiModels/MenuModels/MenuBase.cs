using GalaSoft.MvvmLight;
using System.Xml.Serialization;

namespace FiiiCoin.Wallet.Win.Models
{
    [XmlRoot("MenuBase")]
    [XmlInclude(typeof(MenuInfo))]
    [XmlInclude(typeof(MenuSeparator))]
    public abstract class MenuBase : ViewModelBase
    {
        [XmlAttribute("MenuType")]
        public MenuType MenuType
        {
            get;
            set;
        }
    }

    public enum MenuType
    {
        Item,
        Separator
    }
}