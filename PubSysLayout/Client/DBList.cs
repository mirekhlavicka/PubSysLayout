using System.Net.Http.Json;

namespace PubSysLayout.Client
{
    public class DBList
    {
        private string[] dblist = null;
        private HttpClient httpClient;

        public DBList(HttpClient httpClient)
        { 
            this.httpClient = httpClient;
        }

        public async Task<string[]>  GetList()
        {
            if (dblist == null)
            {
                dblist = await httpClient.GetFromJsonAsync<string[]>("api/dblist");
            }

            return dblist;
        }
    }
}
