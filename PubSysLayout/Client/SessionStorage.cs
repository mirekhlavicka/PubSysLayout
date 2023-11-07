using System.Text.Json;

namespace PubSysLayout.Client
{
    public class SessionStorage
    {
        private Dictionary<string, object> items = null;
        private readonly LocalStorage localStorage;

        public SessionStorage(LocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

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
                return items?.GetValueOrDefault(key); 
            }
            set 
            {
                if (items != null)
                {
                    items[key] = value;
                }
            }
        }

        public async Task Save()
        {
            await localStorage.SetAsync("Session", items);
            await localStorage.SetAsync("Session_Types", items.ToDictionary(i => i.Key, i => i.Value != null ? i.Value.GetType().FullName : "null"));
        }

        private Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;

            return GetType($"{typeName},  PubSysLayout.Shared");
        }

        public async Task Load()
        {
            if (items == null)
            {
                items = await localStorage.GetAsync<Dictionary<string, object>>("Session");
                Dictionary<string, string> types = await localStorage.GetAsync<Dictionary<string, string>>("Session_Types");

                if(items != null  && types != null && items.Count == types.Count) 
                {
                    foreach (var kv in items.ToArray()) 
                    {
                        items[kv.Key] = types[kv.Key] == "null" ? null : ((JsonElement)(kv.Value)).Deserialize(GetType(types[kv.Key]));
                    }
                }

                items ??= new Dictionary<string, object>();
            }
        }
    }
}
