using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class ModuleSetting
    {
        public int IdSetting { get; set; }
        public int IdModule { get; set; }
        public int IdModuleusage { get; set; }
        public int IdServer { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        public virtual Module IdModuleNavigation { get; set; }
    }
}
