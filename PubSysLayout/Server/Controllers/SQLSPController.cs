using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PubSysLayout.Shared.Model;
using PubSysLayout.Shared.SQLQuery;
using PubSysLayout.Shared.SQLSP;
using System.Data;
using System.Text.RegularExpressions;
using static PubSysLayout.Server.Controllers.CodeController;

namespace PubSysLayout.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class SQLSPController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SQLSPController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SPInfo>> GetSPList(string search, bool incode, string database)
        {
            string SQL =
                @"SELECT
	                object_id, name, create_date, modify_date, type
                FROM
	                sys.objects
                WHERE
                    type in ('P','FN','IF','TF','TR') AND
	                (name LIKE '%' + @search + '%'{0})
                ORDER BY
	                name";

            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {
                try
                {
                    using (var cmd = new SqlCommand(String.Format(SQL, incode ? " OR Object_definition(object_id) LIKE '%' + @search + '%'" : ""), conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@search", SqlDbType.NVarChar, 256).Value = search;
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var resultTable = new DataTable();
                            adapter.Fill(resultTable);
                            return Ok(resultTable.AsEnumerable().Select(dr => new SPInfo
                            {
                                Database = database,
                                ObjectId = dr.Field<int>("object_id"),
                                Name = dr.Field<string>("name"),
                                Type = dr.Field<string>("type"), 
                                CreateDate = dr.Field<DateTime>("create_date"),
                                ModifyDate = dr.Field<DateTime>("modify_date")

                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet("objectdefinition")]
        public ActionResult<string> GetObjectDefinition(int object_id, string database)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {

                try
                {
                    using (var cmd = new SqlCommand(@"SELECT Object_definition(@object_id)", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@object_id", SqlDbType.Int).Value = object_id;
                        conn.Open();
                        return Ok((string)cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut]
        public ActionResult<string> SetObjectDefinition(SPInfo sp)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), sp.Database)))
            {

                try
                {
                    sp.Code = Regex.Replace(sp.Code, @"CREATE(\s+)(PROCEDURE|FUNCTION|TRIGGER)", "ALTER$1$2", RegexOptions.IgnoreCase);
                    using (var cmd = new SqlCommand(sp.Code, conn))
                    {
                        cmd.CommandType = CommandType.Text;                        
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return Ok();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
