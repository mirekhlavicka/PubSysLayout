using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PubSysLayout.Shared.SQLQuery;
using Query = PubSysLayout.Shared.SQLQuery.Query;

namespace PubSysLayout.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SQLQueryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private int maxRowCount = 1000;
        public SQLQueryController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            maxRowCount = _configuration.GetValue<int>("SQLQuery:maxRowCount");
        }

        [HttpPost]
        public IActionResult Run(Query query)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), query.Database)))
            {
                try
                {
                    using (var cmd = new SqlCommand(query.SQL, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var resultTable = new DataTable();
                            adapter.Fill(resultTable);
                            return Ok(new QueryResult
                            {
                                Columns = resultTable.Columns.Cast<DataColumn>().Select(dc => new QueryResultColumn 
                                { 
                                    Name = dc.ColumnName, 
                                    TypeName = dc.DataType.ToString() 
                                }).ToArray(),
                                Rows = ConvertToArray(resultTable)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("save")]
        public IActionResult Save(Query query, string name)
        {
            var directoryName = _configuration.GetValue<string>("SQLQuery:savedSQLDirectoryName");
            var directoryPath = Path.Combine(_environment.ContentRootPath, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!String.IsNullOrEmpty(name))
            {
                try
                {
                    string SQL = System.IO.File.ReadAllText(Path.Combine(directoryPath, $"{name}.sql"));

                    if (SQL == query.SQL)
                    {
                        return Ok(name);
                    }
                    else
                    {
                        name = string.Empty;
                    }
                }
                catch
                { 
                    name = string.Empty;
                }
            }

            if (String.IsNullOrEmpty(name))
            {
                name = Guid.NewGuid().ToString("n").Substring(0, 12);
            }

            System.IO.File.WriteAllText(Path.Combine(directoryPath, $"{name}.sql"), query.SQL);

            return Ok(name);
        }

        [HttpGet("savedsql")]
        public string GetSavedSQL(string name)
        {
            var directoryName = _configuration.GetValue<string>("SQLQuery:savedSQLDirectoryName");
            var directoryPath = Path.Combine(_environment.ContentRootPath, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                return "";
            }

            string SQL = "";
            try
            {
                SQL = System.IO.File.ReadAllText(Path.Combine(directoryPath, $"{name}.sql"));
            }
            catch
            { }
            return SQL;
        }

        [HttpGet("dblist")]
        public IEnumerable<string> GetDBList()
        {
            return _configuration.GetSection("SQLQuery:dbList").Get<string[]>();
        }

        [HttpGet("defaultsql")]
        public string GetDefaultSQL()
        {
            return _configuration.GetValue<string>("SQLQuery:defaultSQL");
        }

        [HttpGet("defaultdb")]
        public string GetDefaultDB()
        {
            return _configuration.GetValue<string>("SQLQuery:defaultDB");
        }

        private object[][] ConvertToArray(DataTable dataTable)
        {
            int count = Math.Min(dataTable.Rows.Count, maxRowCount);
            var result = new object[count][];
            for (int p = 0; p < count; p++)
            {
                result[p] = dataTable.Rows[p].ItemArray;
            }
            return result;
        }

        //private static List<dynamic> ConvertToDynamic(DataTable dataTable)
        //{
        //    var result = new List<dynamic>();
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        dynamic dyn = new System.Dynamic.ExpandoObject();
        //        var dic = (IDictionary<string, object>)dyn;
        //        foreach (DataColumn column in dataTable.Columns)
        //        {
        //            dic[column.ColumnName] = row[column];
        //        }
        //        result.Add(dyn);
        //    }
        //    return result;
        //}
    }
}
