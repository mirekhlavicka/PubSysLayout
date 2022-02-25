using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Style
    {
        public Style()
        {
            LayoutDefinitions = new HashSet<LayoutDefinition>();
        }

        public int IdStyle { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Skin { get; set; }

        public virtual ICollection<LayoutDefinition> LayoutDefinitions { get; set; }
    }
}
