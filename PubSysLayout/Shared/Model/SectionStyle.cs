using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class SectionStyle
    {
        public int IdSection { get; set; }
        public int IdStyle { get; set; }
        public bool Propagate { get; set; }
    }
}
