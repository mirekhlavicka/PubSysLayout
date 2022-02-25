using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Section
    {
        public int IdSection { get; set; }
        public int IdMetasection { get; set; }
        public int IdServer { get; set; }
        public int IdSectionParent { get; set; }
        public int IdSectionParentTop { get; set; }
        public int IdFile { get; set; }
        public byte Treelevel { get; set; }
        public string Name { get; set; }
        public string Redirurl { get; set; }
        public byte Target { get; set; }
        public bool Visible { get; set; }
        public bool Del { get; set; }
        public int Order { get; set; }
        public int Options { get; set; }
        public int Tag { get; set; }
    }
}
