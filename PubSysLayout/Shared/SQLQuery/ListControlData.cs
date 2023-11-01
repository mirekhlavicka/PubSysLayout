using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLQuery
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
                if (items == null) //!!! distinct values !!!
                {
                    items = Items.ToDictionary(it => it.Value, it => it.Text);
                }
                string res;
                if (!items.TryGetValue(v, out res))
                {
                    res = "???";
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
