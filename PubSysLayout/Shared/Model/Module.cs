using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Module
    {
        public Module()
        {
            ModuleLocals = new HashSet<ModuleLocal>();
            ModuleSettings = new HashSet<ModuleSetting>();
            ModuleUsages = new HashSet<ModuleUsage>();
        }

        public int IdModule { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DesktopSrc { get; set; }
        public string MobileSrc { get; set; }
        public bool Admin { get; set; }
        public bool Active { get; set; }
        public string Qskey { get; set; }

        public virtual ICollection<ModuleLocal> ModuleLocals { get; set; }
        public virtual ICollection<ModuleSetting> ModuleSettings { get; set; }
        public virtual ICollection<ModuleUsage> ModuleUsages { get; set; }
    }
}
