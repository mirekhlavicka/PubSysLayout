﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PubSysLayout.Shared.SQLQuery;
using Query = PubSysLayout.Shared.SQLCatalog.Query;
using PubSysLayout.Shared.SQLCatalog;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _memoryCache;
        private int maxRowCount = 200;
        public SQLCatalogController(IConfiguration configuration, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _environment = environment;
            _memoryCache = memoryCache;
            maxRowCount = _configuration.GetValue<int>("CatalogQuery:maxRowCount", 200);
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public IActionResult Run(Query query)
        {
            try
            {
                using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), query.Database)))
                {
                    var formControls = GetData($"SELECT * FROM FormControls WHERE id_form={query.IdForm} ORDER BY sortorder", conn);

                    var dtItems = GetData(BuildSQL(query, formControls), conn);
                    var items = ConvertToArray(dtItems);
                    var serverName = (string)GetData("SELECT TOP 1 server_name FROM ServerNames WHERE [default] = 1", conn).Rows[0][0];
                    var listData = GetListControlData(query.Database, query.IdForm, serverName);
                    var shown = formControls.AsEnumerable().Where(fc => query.Include.Contains(fc.Field<int>("id_fcontrol"))).ToArray();
                    for (int i = 0; i < shown.Length; i++)
                    {
                        int id_fcontrol = shown[i].Field<int>("id_fcontrol");
                        if (listData.ContainsKey(id_fcontrol))
                        {
                            foreach (var r in items)
                            {
                                if (listData[id_fcontrol].Multival)
                                {
                                    r[i + 2] = String.Join(", ", r[i + 2]?.ToString().Split(',').Select(v => listData[id_fcontrol][v.Trim()]));
                                }
                                else
                                {
                                    r[i + 2] = listData[id_fcontrol][r[i + 2]?.ToString()];
                                }
                            }
                        }
                        else if (shown[i].Field<byte>("datatype") == 5)
                        {
                            foreach (var r in items)
                            {
                                r[i + 2] = (int)r[i + 2] > 0 ? $"https://{serverName}/getfile.aspx?id_file={r[i + 2]}" : null;
                            }
                        }
                    }

                    return Ok(new QueryResult
                    {
                        TableName = "",
                        Columns = dtItems.Columns.Cast<DataColumn>().Select((dc, i) => new QueryResultColumn
                        {
                            Name = dc.ColumnName,
                            TypeName = (i > 1 && shown[i - 2].Field<byte>("datatype") == 5 ? "System.String" : dc.DataType.ToString()),
                            ReadOnly = dc.ReadOnly,
                            MaxLength = dc.MaxLength,
                            AllowDBNull = dc.AllowDBNull
                        }).ToArray(),
                        Rows = items
                    });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sql")]
        public string GetSQL(Query query)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), query.Database)))
            {
                var formControls = GetData($"SELECT * FROM FormControls WHERE id_form={query.IdForm} ORDER BY sortorder", conn);

                return BuildSQL(query, formControls);
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

        [HttpGet("listcontroldata")]
        public Dictionary<int, ListControlData> GetListControlData(string database, int id_form, string serverName)
        {
            string cacheKey = $"ListControlData_{database}_{id_form}";

            if (_memoryCache.TryGetValue(cacheKey, out Dictionary<int, ListControlData> result))
            { 
                return result;
            }

            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            using (var httpClient = httpClientFactory.CreateClient())
            {
                var formControls = GetData($"SELECT * FROM FormControls WHERE id_form={id_form} ORDER BY sortorder", conn);

                if (String.IsNullOrEmpty(serverName))
                {
                    serverName = (string)GetData("SELECT TOP 1 server_name FROM ServerNames WHERE [default] = 1", conn).Rows[0][0];
                }

                result = formControls.AsEnumerable()
                    .Where(dr => new int[] { 2, 3, 4, 5 }.Contains(dr.Field<int>("id_control")))
                    .Select(dr => dr.Field<int>("id_fcontrol"))
                    .ToDictionary(id => id, id => httpClient.GetFromJsonAsync<ListControlData>($"https://{serverName}/systools/FormControlData.ashx?id_fcontrol={id}").Result);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(1800));
                _memoryCache.Set(cacheKey, result, cacheEntryOptions);

                return result;
            }
        }


        private string BuildSQL(Query query, DataTable formControls)
        {
            var selectList = String.Join(",\r\n", formControls.AsEnumerable().Where(fc => query.Include.Contains(fc.Field<int>("id_fcontrol"))).Select(dr => $"\tfif{dr.Field<int>("id_fcontrol")}." + dr.Field<byte>("datatype") switch
            {
                0 => "strvalue",
                1 or 5 or 6 or 8 or 9 => "intvalue",
                2 => "numvalue",
                3 => "datevalue",
                10 => "richvalue",
                _ => "strvalue"
            } + $" AS [{dr.Field<string>("title").Replace("[", "/").Replace("]", "/")}]"));

            var joinList = String.Join(" ", formControls.AsEnumerable()
                .Where(fc => query.Include.Contains(fc.Field<int>("id_fcontrol")) || query.Where.Any(kv => !String.IsNullOrEmpty(kv.Value) && kv.Key == fc.Field<int>("id_fcontrol")))
                .Select(dr => $"LEFT JOIN\r\n\tFormItemFields fif{dr.Field<int>("id_fcontrol")} ON fi.id_item=fif{dr.Field<int>("id_fcontrol")}.id_item AND fif{dr.Field<int>("id_fcontrol")}.id_fcontrol={dr.Field<int>("id_fcontrol")}"));

            var where = $"\r\n\tfi.id_form={query.IdForm}";

            if (query.Id.HasValue)
            {
                where += $" AND\r\n\tfi.id_item = {query.Id}";
            }

            if (query.Released.HasValue)
            {
                where += $" AND\r\n\tfi.released = {(query.Released.Value ? "1" : "0")}";
            }

            if (query.Where.Values.Any(v => !String.IsNullOrEmpty(v)))
            {
                where += " AND\r\n\t" + String.Join(" AND\r\n\t", query.Where
                    .Where(kv => !String.IsNullOrEmpty(kv.Value))
                    .Select(kv => {
                        var fc = formControls.AsEnumerable().Single(dr => dr.Field<int>("id_fcontrol") == kv.Key);
                        if (fc.Field<bool>("parse_multival"))
                        {
                            return $"fi.id_item IN (SELECT fif.id_item FROM FormItemFieldsMultival mv JOIN FormItemFields fif ON mv.id_field=fif.id_field WHERE fif.id_fcontrol={kv.Key} AND mv.intvalue={kv.Value})";
                        }
                        else
                        {
                            return $"fif{kv.Key}." + fc.Field<byte>("datatype") switch
                            {
                                0 => "strvalue" + $" {(kv.Value.Contains('%') ? "LIKE" : "=")} '{kv.Value}'",
                                1 or 5 or 6 or 8 or 9 => "intvalue" + $" = {kv.Value}",
                                2 => "numvalue" + $" = {kv.Value}",
                                3 => "datevalue" + $" = '{kv.Value}'",
                                10 => "richvalue" + $" = '{kv.Value}'",
                                _ => "strvalue" + $" = '{kv.Value}'"
                            };
                        }
                    }));
            }

            return $"SELECT TOP {maxRowCount}\r\n\tfi.id_item AS [Id],\r\n\tfi.released AS [Released],\r\n{selectList}\r\nFROM\r\n\tFormItems fi {joinList}\r\nWHERE{where}\r\nORDER BY\r\n\tfi.id_item DESC";
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