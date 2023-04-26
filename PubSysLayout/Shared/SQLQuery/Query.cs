using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLQuery
{
    public class Query
    {
        public string Database { get; set; }
        public string SQL { get; set; }
        public object[] OriginalRow { get; set; }
        public object[] Row { get; set; }
        public string Action { get; set; }
    }
}
