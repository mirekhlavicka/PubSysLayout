using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLSP
{
    public class SPInfo
    {
        public string Database { get; set; }
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
    }
}
