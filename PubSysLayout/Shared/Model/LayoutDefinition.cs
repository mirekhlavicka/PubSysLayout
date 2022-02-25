using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class LayoutDefinition
    {
        public LayoutDefinition()
        {
            LayoutAssigns = new HashSet<LayoutAssign>();
            ModuleUsages = new HashSet<ModuleUsage>();
        }

        public int IdLayoutdefinition { get; set; }
        public int IdLayout { get; set; }
        public int IdStyle { get; set; }
        public bool Mainstyle { get; set; }
        public string Name { get; set; }

        public virtual Layout IdLayoutNavigation { get; set; }
        public virtual Style IdStyleNavigation { get; set; }
        public virtual ICollection<LayoutAssign> LayoutAssigns { get; set; }
        public virtual ICollection<ModuleUsage> ModuleUsages { get; set; }
    }
}
