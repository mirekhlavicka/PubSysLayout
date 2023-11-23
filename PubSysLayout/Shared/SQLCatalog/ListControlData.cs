using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLCatalog
{
    public class ListControlData
    {
        public bool Multival { get; set; }
        public Item[] Items { get; set; }

        private Dictionary<string, string> items = null;
        public string this[string v]
        {
            get 
            {
                if (items == null)
                {
                    items = Items.GroupBy(it => it.Value).ToDictionary(it => it.Key, it => it.First().Text);
                }
                string res;
                if (!items.TryGetValue(v, out res))
                {
                    res = String.IsNullOrEmpty(v) ? "" : "???";
                };
                return res;
            }
        }
    }

    public class Item
    {
        public string Value { get; set; }
        public string Text { get; set; } 
    }
}
