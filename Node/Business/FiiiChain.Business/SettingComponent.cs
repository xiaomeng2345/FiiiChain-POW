using FiiiChain.Messages;
using FiiiChain.Entities;
using FiiiChain.DataAgent;
using System;
using System.Collections.Generic;
using System.Text;
using FiiiChain.Data;
using FiiiChain.Framework;
using FiiiChain.Consensus;
using System.Linq;

namespace FiiiChain.Business
{
    public class SettingComponent
    {
        public void SaveSetting(Setting setting)
        {
            new SettingDac().SaveSetting(setting);
        }

        public Setting GetSetting()
        {
            var setting = new SettingDac().GetSetting();

            if(setting == null)
            {
                setting = new Setting();
                setting.Confirmations = 1;
                setting.FeePerKB = 100000;
            }

            return setting;
        }
    }
}
