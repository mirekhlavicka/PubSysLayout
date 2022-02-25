using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Qslayout
    {
        public Qslayout()
        {
            LayoutAssigns = new HashSet<LayoutAssign>();
        }

        public int IdQslayout { get; set; }
        public string ParamString { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool Public { get; set; }

        public virtual ICollection<LayoutAssign> LayoutAssigns { get; set; }
    }
}
