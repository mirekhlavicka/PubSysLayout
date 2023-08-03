using System.Collections;

namespace PubSysLayout.Client
{
    public class TableUsage
    {
        private readonly LocalStorage localStorage;

        private Dictionary<string, int> usage;

        public TableUsage(LocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }


        public async Task Add(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                await Load();
                if (usage.ContainsKey(name))
                {
                    usage[name]++;
                }
                else
                {
                    usage[name] = 1;
                }
                await Save();
            }
        }

        public async Task<Dictionary<string, int>> Load()
        { 
            if(usage == null) 
            {
                usage = await localStorage.GetAsync<Dictionary<string, int>>("TableUsage");

                usage ??= new Dictionary<string, int>();
            }
            return usage;
        }

        private async Task Save()
        {
            if (usage != null)
            {
                await localStorage.SetAsync("TableUsage", usage);
            }
        }
    }
}
