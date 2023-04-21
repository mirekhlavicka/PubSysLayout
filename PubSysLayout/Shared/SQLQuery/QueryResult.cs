using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLQuery
{
    public class QueryResult
    {
        public QueryResultColumn[] Columns { get; set; }   
        public object[][] Rows { get; set; }
    }

    public class QueryResultColumn 
    { 
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool ReadOnly { get; set; }
    }

}
