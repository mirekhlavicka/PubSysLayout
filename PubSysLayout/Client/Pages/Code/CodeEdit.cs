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

        public async Task<string> SelectFTP()
        {
            if (currentDB.FTP == null)
            {
                var ftpresult = await DialogService.Show<Code.SelectFTPDialog>("Select FTP source",
                    new DialogOptions()
                    {
                        MaxWidth = MaxWidth.Small,
                        CloseButton = true
                    }
                    ).Result;

                if (ftpresult.Canceled)
                {
                    return null;
                }
                currentDB.FTP = ftpresult.Data.ToString();
                currentDB.CurrentFTPPath = "~";
            }

            return currentDB.FTP;
        }

        public async Task Edit(string path, string ext = "")
        {
            if (await SelectFTP() == null)
            {
                return;
            }

            path = path.Split('?')[0];

            if (!path.StartsWith("~"))
            {
                if (!path.StartsWith("/"))
                {
                    path = "/" + path;
                }
                path = "~" + path;
            }

            string code = await httpClient.GetStringAsync($"api/code/?ftp={WebUtility.UrlEncode(currentDB.FTP)}&path={WebUtility.UrlEncode(path)}");

            var dialog = DialogService.Show<CodeDialog>(path.Replace("~", currentDB.FTP),
                new DialogParameters
                {
                    ["Code"] = code,
                    ["Path"] = path,
                    ["Ext"] = ext
                },
                new DialogOptions()
                {
                    MaxWidth = MaxWidth.ExtraExtraLarge,
                    FullWidth = true,
                    CloseButton = false,
                    CloseOnEscapeKey = false
                });

            await dialog.Result;
        }

        public async Task EditSQL(string database, string name, string code, Func<string, string, Task<bool>> saveCode)
        {

            var dialog = DialogService.Show<CodeDialog>($"{database}/{name}" ,
                new DialogParameters
                {
                    ["Code"] = code,
                    ["Path"] = "",
                    ["Ext"] = ".sql",
                    ["SaveCode"] = saveCode
                },
                new DialogOptions()
                {
                    MaxWidth = MaxWidth.ExtraExtraLarge,
                    FullWidth = true,
                    CloseButton = false,
                    CloseOnEscapeKey = false
                });

            await dialog.Result;
        }

        public async Task EditCode(string title, string code, Func<string, string, Task<bool>> saveCode)
        {

            var dialog = DialogService.Show<CodeDialog>(title,
                new DialogParameters
                {
                    ["Code"] = code,
                    ["Path"] = "",
                    ["Ext"] = ".aspx",
                    ["SaveCode"] = saveCode,
                    ["CloseOnSave"] = true
                },
                new DialogOptions()
                {
                    MaxWidth = MaxWidth.ExtraExtraLarge,
                    FullWidth = true,
                    CloseButton = false,
                    CloseOnEscapeKey = false
                });

            await dialog.Result;
        }
    }
}
