using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data;

namespace PubSysLayout.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SQLQueryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SQLQueryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Run(PubSysLayout.Shared.SQLQuery.Query query)
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
                            return Ok(ConvertToDynamic(resultTable).Take(10000)); //!!! max rowcount protection !!!
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("dblist")]
        public IEnumerable<string> GetDBList()
        {
            return _configuration.GetSection("dbList").Get<string[]>();
        }


        private static List<dynamic> ConvertToDynamic(DataTable dataTable)
        {
            var result = new List<dynamic>();
            foreach (DataRow row in dataTable.Rows)
            {
                dynamic dyn = new System.Dynamic.ExpandoObject();
                var dic = (IDictionary<string, object>)dyn;
                foreach (DataColumn column in dataTable.Columns)
                {
                    dic[column.ColumnName] = row[column];
                }
                result.Add(dyn);
            }
            return result;
        }
    }
}
