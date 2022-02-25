using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class ModuleUsageParam
    {
        public int IdModuleusage { get; set; }
        public string Param { get; set; }
        public bool Pvalue { get; set; }

        public virtual ModuleUsage IdModuleusageNavigation { get; set; }
    }
}
