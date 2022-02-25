using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Layout
    {
        public Layout()
        {
            LayoutDefinitions = new HashSet<LayoutDefinition>();
        }

        public int IdLayout { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DesktopSrc { get; set; }
        public string MobileSrc { get; set; }

        public virtual ICollection<LayoutDefinition> LayoutDefinitions { get; set; }
    }
}
