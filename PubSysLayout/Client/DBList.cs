using PubSysLayout.Client.AuthProviders;
using System.Net.Http.Json;

namespace PubSysLayout.Client
{
    public class DBList
    {
        private string[] dblist = null;
        private HttpClient httpClient;

        public DBList(HttpClient httpClient, AuthStateProvider authStateProvider)
        { 
            this.httpClient = httpClient;
            authStateProvider.AuthenticationStateChanged += AuthStateProvider_AuthenticationStateChanged;
        }

        private void AuthStateProvider_AuthenticationStateChanged(Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> task)
        {
            dblist = null;
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
