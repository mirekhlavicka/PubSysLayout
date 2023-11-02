using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PubSysLayout.Shared.SQLQuery;
using PubSysLayout.Shared.CatalogQuery;
using Query = PubSysLayout.Shared.SQLQuery.Query;
using PubSysLayout.Shared.SQLCatalog;

namespace PubSysLayout.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SQLCatalogController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpClientFactory httpClientFactory;
        private int maxRowCount = 1000;
        public SQLCatalogController(IConfiguration configuration, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _environment = environment;
            maxRowCount = _configuration.GetValue<int>("SQLQuery:maxRowCount");
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public IActionResult Run(Query query)
        {
            int id_form;

            if (Int32.TryParse(query.SQL, out id_form))
            {
                return Ok(QueryCatalog(query, id_form));
            }
            else
            {
                return BadRequest("Invalid id of catalog");
            }
        }

        private QueryResult QueryCatalog(Query query, int id_form)
        {
            using (var httpClient = httpClientFactory.CreateClient())
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), query.Database)))
            {
                var formControls = GetData($"SELECT * FROM FormControls WHERE id_form={id_form} AND cat_showinlist=1 ORDER BY sortorder", conn);
                var serverName = GetData("SELECT TOP 1 server_name FROM ServerNames WHERE [default] = 1", conn).Rows[0][0];

                var selectList = String.Join(",", formControls.AsEnumerable().Select(dr => $"fif{dr.Field<int>("id_fcontrol")}." + dr.Field<byte>("datatype") switch
                {
                    0 => "strvalue",
                    1 or 5 or 6 or 8 or 9 => "intvalue",
                    3 => "datevalue",
                    10 => "richvalue",
                    _ => "strvalue"
                } + $" AS [{dr.Field<string>("title")}]"));
                var joinList = String.Join("\r\n", formControls.AsEnumerable().Select(dr => $"LEFT JOIN FormItemFields fif{dr.Field<int>("id_fcontrol")} ON fi.id_item=fif{dr.Field<int>("id_fcontrol")}.id_item AND fif{dr.Field<int>("id_fcontrol")}.id_fcontrol={dr.Field<int>("id_fcontrol")}"));
                var dtItems = GetData($"SELECT TOP 200 fi.id_item, fi.released AS [Publikován], {selectList} FROM FormItems fi {joinList} WHERE fi.id_form={id_form} ORDER BY id_item DESC", conn);
                var items = ConvertToArray(dtItems);

                var listData = formControls.AsEnumerable()
                    .Where(dr => new int[] { 2, 3, 4, 5 }.Contains(dr.Field<int>("id_control")))
                    .Select(dr => dr.Field<int>("id_fcontrol"))
                    .ToDictionary(id => id, id => httpClient.GetFromJsonAsync<ListControlData>($"https://{serverName}/systools/FormControlData.ashx?id_fcontrol={id}").Result);

                for (int i = 0; i < formControls.Rows.Count; i++)
                {
                    int id_fcontrol = (int)formControls.Rows[i]["id_fcontrol"];
                    if (listData.ContainsKey(id_fcontrol) && !listData[id_fcontrol].Multival)
                    {
                        foreach (var r in items)
                        {
                            r[i + 2] = listData[id_fcontrol][r[i + 2]?.ToString()];
                        }
                    }
                }

                return new QueryResult
                {
                    TableName = "",
                    Columns = dtItems.Columns.Cast<DataColumn>().Select(dc => new QueryResultColumn
                    {
                        Name = dc.ColumnName,
                        TypeName = dc.DataType.ToString(),
                        ReadOnly = dc.ReadOnly,
                        MaxLength = dc.MaxLength,
                        AllowDBNull = dc.AllowDBNull
                    }).ToArray(),
                    Rows = items
                };
            }
        }

        [HttpGet("catlist")]
        public IEnumerable<KeyValuePair<int, string>> GetCatalogList(string database)
        {
            return GetData(@"SELECT id_form, catalogname FROM Catalogs WHERE pub_only = 0 ORDER BY id_catalog", database)
                .AsEnumerable()
                .Select(dr => new KeyValuePair<int, string>(dr.Field<int>("id_form"), dr.Field<string>("catalogname")));
        }

        [HttpGet("formcontrols")]
        public IEnumerable<FormControl> GetFormControls(string database, int id_form)
        {
            return GetData(@$"
                        SELECT
                            id_fcontrol, id_control, title, sortorder, required, datatype, searchable, sortable, cat_showinlist
                        FROM
                            FormControls
                        WHERE
                            id_form = {id_form}
                        ORDER BY
                            sortorder", database)
                .AsEnumerable()
                .Select(dr => new FormControl 
                { 
                    IdControl = dr.Field<int>("id_control"),
                    IdFControl = dr.Field<int>("id_fcontrol"),
                    Title = dr.Field<string>("Title"),
                    ShowInList = dr.Field<bool>("cat_showinlist"),
                    Searchable = dr.Field<bool> ("searchable"),
                    DataType = dr.Field<byte>("datatype")
                });
        }

        private DataTable GetData(string sql, string database)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {
                return GetData(sql, conn);
            }
        }

        private DataTable GetData(string sql, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(sql);
            DataTable res = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                sda.Fill(res);
            }
            finally
            {
                cmd.Dispose();
            }
            return res;
        }

        private object[][] ConvertToArray(DataTable dataTable)
        {
            var result = new object[dataTable.Rows.Count][];
            for (int p = 0; p < dataTable.Rows.Count; p++)
            {
                result[p] = dataTable.Rows[p].ItemArray;
            }
            return result;
        }
    }
}
