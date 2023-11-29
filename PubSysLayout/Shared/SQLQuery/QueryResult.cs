using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLQuery
{
    public class QueryResult
    {
        public string TableName { get; set; }
        public QueryResultColumn[] Columns { get; set; }   
        public List<object[]> Rows { get; set; }
    }

    public class QueryResultColumn 
    { 
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool ReadOnly { get; set; }
        public int MaxLength { get; set; }
        public bool AllowDBNull { get; set; }
    }

}
