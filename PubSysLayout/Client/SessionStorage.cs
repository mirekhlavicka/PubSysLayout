namespace PubSysLayout.Client
{
    public class SessionStorage
    {
        private Dictionary<string, object> items = new Dictionary<string, object>();

        public Dictionary<string, object> Items
        {
            get
            {
                return items;
            }
        }

        public object this[string key]
        {
            get 
            { 
                return items.GetValueOrDefault(key); 
            }
            set 
            { 
                items[key] = value; 
            }
        }
    }
}
