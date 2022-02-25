using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Spot
    {
        public Spot()
        {
            ModuleUsages = new HashSet<ModuleUsage>();
        }

        public int IdSpot { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ModuleUsage> ModuleUsages { get; set; }
    }
}
