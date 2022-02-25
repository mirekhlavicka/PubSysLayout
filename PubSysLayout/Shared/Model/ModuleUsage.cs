using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class ModuleUsage
    {
        public ModuleUsage()
        {
            ModuleUsageParams = new HashSet<ModuleUsageParam>();
        }

        public int IdModuleusage { get; set; }
        public int IdModule { get; set; }
        public int IdLayoutdefinition { get; set; }
        public int Order { get; set; }
        public int IdSpot { get; set; }
        public int CacheTime { get; set; }
        public bool ShowMobile { get; set; }

        public virtual LayoutDefinition IdLayoutdefinitionNavigation { get; set; }
        public virtual Module IdModuleNavigation { get; set; }
        public virtual Spot IdSpotNavigation { get; set; }
        public virtual ICollection<ModuleUsageParam> ModuleUsageParams { get; set; }
    }
}
