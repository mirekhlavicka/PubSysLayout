using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLCatalog
{
    public class FormControl
    {
        public int IdControl { get; set; }
        public int IdFControl { get; set; }
        public string Title { get; set; }
        public int DataType { get; set; }
        public bool Searchable { get; set; }
        public bool ShowInList { get; set; }

        public bool Required { get; set; }
        public bool MultiLine { get; set; } 
        public int MaxLength { get; set; }
        public bool Multival { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
}
}
