using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLCatalog
{
    public class Query
    {
        public string Database { get; set; }
        public int IdForm { get; set; }

        public HashSet<int> Include { get; set; }

        public int? Id {  get; set; }

        public bool? Released { get; set; }

        public Dictionary<int, string> Where { get; set; }

        public int MaxRowCount { get; set; }

    }
}
