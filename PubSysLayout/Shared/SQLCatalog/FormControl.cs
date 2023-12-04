using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLCatalog
{
    public class FormControl
    {
        public int IdControl { get; set; }
        public int IdFControl { get; set; }
        public int IdTabPage { get; set; }
        public string Title { get; set; }
        public int DataType { get; set; }
        public bool Searchable { get; set; }
        public bool ShowInList { get; set; }
        public bool Required { get; set; }
        public bool MultiLine { get; set; }
        public int MaxLength { get; set; }
        public bool Multival { get; set; }
        public bool ParseMultival { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [JsonIgnore]
        public Type Type
        {
            get
            {
                switch (DataType)
                {
                    case 0:
                    case 10:
                        return typeof(string);
                    case 1:
                    case 5:
                    case 6:
                    case 8:
                    case 9:
                        return typeof(int);
                    case 2:
                    case 4:
                        return typeof(decimal);
                    case 3:
                        return typeof(DateTime);
                    default:
                        return null;
                }
            }
        }
    }
}
