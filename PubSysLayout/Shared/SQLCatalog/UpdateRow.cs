using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLCatalog
{
    public class UpdateRow
    {
        public string Database { get; set; }
        public int IdForm { get; set; }
        public int IdItem { get; set; }        
        public object[] Row { get; set; }
        public bool Released { get; set; }
        public HashSet<int> Include { get; set; }
    }
}
