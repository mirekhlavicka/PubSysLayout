using MudBlazor;
using System.Net;
using System.Net.Http.Json;

namespace PubSysLayout.Client.Pages.Code
{
    public class CodeEdit
    {
        private HttpClient httpClient;
        private IDialogService DialogService;
        private CurrentDB currentDB;

        public CodeEdit(HttpClient httpClient, IDialogService DialogService, CurrentDB currentDB)
        {
            this.httpClient = httpClient;
            this.DialogService = DialogService;
            this.currentDB = currentDB;
        }

        public async void Edit(string path)
        {
            if (currentDB.FTP == null)
            {
                var ftpresult = await DialogService.Show<Code.SelectFTPDialog>("Select FTP source",
                    //new DialogParameters { },
                    new DialogOptions()
                    {
                        MaxWidth = MaxWidth.Small,
                        CloseButton = true
                    }
                    ).Result;

                if (ftpresult.Cancelled)
                {
                    return;
                }
                currentDB.FTP = ftpresult.Data.ToString();
            }

            string code = await httpClient.GetStringAsync($"/api/code/?ftp={WebUtility.UrlEncode(currentDB.FTP)}&path={WebUtility.UrlEncode(path)}");

            DialogService.Show<CodeDialog>(path.Replace("~", currentDB.FTP),
                new DialogParameters
                {
                    ["Code"] = code,
                    ["Path"] = path
                },
                new DialogOptions()
                {
                    MaxWidth = MaxWidth.ExtraExtraLarge,
                    FullWidth = true,
                    CloseButton = false,
                    CloseOnEscapeKey = false
                });
        }
    }
}
