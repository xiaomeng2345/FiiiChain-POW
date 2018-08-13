using System.Xml.Serialization;

namespace FiiiCoin.Wallet.Win.Models
{

    [XmlRoot("MenuSeparator")]
    public class MenuSeparator : MenuBase
    {
        public MenuSeparator()
        {
            this.MenuType = MenuType.Separator;
        }
    }
}
