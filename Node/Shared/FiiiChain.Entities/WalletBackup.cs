using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class WalletBackup
    {
        public List<Account> AccountList { get; set; }

        public List<AddressBookItem> AddressBookItemList { get; set; }

        public List<Setting> SettingList { get; set; }

        public List<TransactionComment> TransactionCommentList { get; set; }
    }
}
