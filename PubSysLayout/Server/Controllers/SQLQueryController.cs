﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PubSysLayout.Shared.SQLQuery;
using Query = PubSysLayout.Shared.SQLQuery.Query;
using System.Text.RegularExpressions;
using System.Text.Json;

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
            maxRowCount = _configuration.GetValue<int>("SQLQuery:maxRowCount", 1000);
        }

        [HttpPost]
        public IActionResult Run(Query query)
        {
            foreach (string s in _configuration.GetSection("SQLQuery:disabledCommands").Get<string[]>())
            {
                if (Regex.Match(query.SQL, s + "(?=\\s(?!@))", RegexOptions.IgnoreCase).Success)
                {
                    return BadRequest("Invalid command");
                }
            }
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), query.Database)))
            {
                try
                {
                    using (var cmd = new SqlCommand(query.SQL, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            if (query.AllowUpdate)
                            {
                                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                            }                            
                            DataTable table = new DataTable();
                            adapter.Fill(0, maxRowCount, table);
                            return Ok(new QueryResult
                            {
                                TableName = query.AllowUpdate ? table.TableName : "",
                                Columns = table.Columns.Cast<DataColumn>().Select(dc => new QueryResultColumn 
                                { 
                                    Name = dc.ColumnName, 
                                    TypeName = dc.DataType.ToString(),
                                    ReadOnly = dc.ReadOnly,
                                    MaxLength = dc.MaxLength,
                                    AllowDBNull = dc.AllowDBNull
                                }).ToArray(),
                                Rows = ConvertToArray(table)
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

        [HttpPut] 
        public IActionResult UpdateRow(Query query) 
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), query.Database)))
            {
                try
                {
                    using (var cmd = new SqlCommand(query.SQL, conn)) 
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                            DataTable table = new DataTable();
                            adapter.FillSchema(table, SchemaType.Source);
                            DataRow srcRow = FillDataRow(table, query.Row);
                            if (query.Action == "UPDATE")
                            {
                                DataRow targetRow = FillDataRow(table, query.OriginalRow);
                                table.Rows.Add(targetRow);
                                table.AcceptChanges();
                                for (int i = 0; i < query.Row.Length; i++)
                                {
                                    if (table.Columns[i].ReadOnly)
                                    {
                                        continue;
                                    }
                                    targetRow[i] = srcRow[i];
                                }
                            }
                            else if (query.Action == "INSERT")
                            {
                                table.Rows.Add(srcRow);
                                string identity_name = null;
                                try
                                {
                                    identity_name = table.Columns.Cast<DataColumn>().SingleOrDefault(c => c.AutoIncrement).ColumnName;
                                }
                                catch { }

                                if (identity_name != null)
                                {
                                    adapter.InsertCommand = builder.GetInsertCommand().Clone();
                                    adapter.InsertCommand.CommandText += $"; SELECT * FROM {table.TableName} WHERE {identity_name} = SCOPE_IDENTITY()";
                                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
                                }
                            }
                            else if (query.Action == "DELETE")
                            {
                                table.Rows.Add(srcRow);
                                table.AcceptChanges();
                                srcRow.Delete();
                            }
                            adapter.Update(table);
                            table.AcceptChanges();
                            return Ok(query.Action == "INSERT" ? srcRow.ItemArray : null);
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
        public IActionResult Save(Query query, string name, bool readOnly)
        {
            string directoryPath = GetDirectoryPath();

            if (!String.IsNullOrEmpty(name))
            {
                try
                {
                    string filePath = Path.Combine(directoryPath, $"{name}.sql");
                    bool isReadOnly = ((System.IO.File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                    string SQL = System.IO.File.ReadAllText(filePath);
                    if (SQL == query.SQL)
                    {
                        if (readOnly && !isReadOnly)
                        {
                            System.IO.File.SetAttributes(filePath, FileAttributes.ReadOnly);
                        }
                        return Ok(name);
                    }
                    else if (isReadOnly) 
                    {
                        name = null;
                    }
                }
                catch
                {
                    name= null;
                }
            }

            if (String.IsNullOrEmpty(name))
            {
                name = Guid.NewGuid().ToString("n").Substring(0, 12);
            }
            
            System.IO.File.WriteAllText(Path.Combine(directoryPath, $"{name}.sql"), query.SQL);
            if (readOnly)
            {
                System.IO.File.SetAttributes(Path.Combine(directoryPath, $"{name}.sql"), FileAttributes.ReadOnly);
            }
            return Ok(name);
        }

        [HttpGet("savedsql")]
        public string GetSavedSQL(string name)
        {
            string SQL = "";
            try
            {
                SQL = System.IO.File.ReadAllText(Path.Combine(GetDirectoryPath(), $"{name}.sql"));
            }
            catch
            { }
            return SQL;
        }

        [HttpPost("favorites")]
        public IActionResult SaveFavorites([FromBody] object data)
        {
            System.IO.File.WriteAllText(Path.Combine(GetDirectoryPath(), $"{User.Identity.Name}.json"), data.ToString());
            return Ok();
        }

        [HttpGet("favorites")]
        public string GetFavorites()
        {
            try
            {
                string res = System.IO.File.ReadAllText(Path.Combine(GetDirectoryPath(), $"{User.Identity.Name}.json"));
                return res;
            }
            catch 
            {
                return "{}";
            }
        }

        [HttpGet("searchtable")]
        public IEnumerable<string> SearchTable(string database, string search, bool searchinside)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {
                using (var cmd = new SqlCommand(
                                            $@"SELECT TOP 50
                                                    TABLE_NAME
                                                FROM
                                                    INFORMATION_SCHEMA.TABLES
                                                {(String.IsNullOrEmpty(search) ? "" : "WHERE TABLE_NAME LIKE " + (searchinside ? "'%' + " : "") + "@search + '%'")}
                                                ORDER BY
	                                                TABLE_NAME", conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        cmd.Parameters.Add("@search", SqlDbType.VarChar, 255).Value = search ?? "";
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table.AsEnumerable().Select(dr => dr[0].ToString());
                    }
                }
            }
        }

        [HttpGet("tablecolumns")]
        public string[][] TableColumns(string database, string tableName)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {
                using (var cmd = new SqlCommand(
                                            @"SELECT
                                                COLUMN_NAME--, ORDINAL_POSITION, DATA_TYPE
                                            FROM
                                                INFORMATION_SCHEMA.COLUMNS
                                            WHERE
                                                TABLE_NAME = @tablename
                                            ORDER BY ORDINAL_POSITION;

                                            SELECT t.COLUMN_NAME FROM
											(
												SELECT DISTINCT
													C.COLUMN_NAME 
												FROM  
													INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN 
													INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME  
												WHERE
													C.TABLE_NAME=@tablename AND T.CONSTRAINT_TYPE IN ('FOREIGN KEY',  'PRIMARY KEY')
											) t JOIN INFORMATION_SCHEMA.COLUMNS c ON c.COLUMN_NAME=t.COLUMN_NAME AND c.TABLE_NAME=@tablename
											ORDER BY
												c.ORDINAL_POSITION", conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        cmd.Parameters.Add("@tablename", SqlDbType.VarChar, 255).Value = tableName;
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return new string[][] 
                        {
                            ds.Tables[0].AsEnumerable().Select(dr => dr[0].ToString()).ToArray(),
                            ds.Tables[1].AsEnumerable().Select(dr => dr[0].ToString()).ToArray()
                        };
                    }
                }
            }
        }

        [HttpGet("tableforeignkeys")]
        public string[][] TableForeignKeys(string database, string tableNames)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {
                using (var cmd = new SqlCommand(String.Format(
                                            @"SELECT  
                                                fk.name,
                                                OBJECT_NAME(fk.parent_object_id) 'parent_table',
                                                OBJECT_NAME(fk.referenced_object_id) 'referenced_table'
                                            FROM 
                                                sys.foreign_keys fk
                                            WHERE
	                                            OBJECT_NAME(fk.parent_object_id) IN ({0}) OR
	                                            OBJECT_NAME(fk.referenced_object_id) IN ({0})
                                            ORDER BY
	                                            OBJECT_NAME(fk.parent_object_id),
	                                            OBJECT_NAME(fk.referenced_object_id)", "'" + tableNames.Replace(",", "','") + "'"), conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        //cmd.Parameters.Add("@tablename", SqlDbType.VarChar, 255).Value = tableName;
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt.AsEnumerable().Select(dr => new string[] 
                        {
                            dr[0].ToString(),
                            dr[1].ToString(),
                            dr[2].ToString()
                        }).ToArray();
                    }
                }
            }
        }

        [HttpGet("foreignkeycolumns")]
        public string[][] ForeignKeyColumns(string database, string fkName)
        {
            using (var conn = new SqlConnection(String.Format(_configuration.GetConnectionString("PubSysDefault"), database)))
            {
                using (var cmd = new SqlCommand(
                                            @"SELECT
                                                OBJECT_NAME(fk.parent_object_id) 'parent_table',
                                                c1.name 'parent_column',
                                                OBJECT_NAME(fk.referenced_object_id) 'referenced_table',
                                                c2.name 'referenced_column'
                                            FROM 
	                                            sys.foreign_key_columns fkc JOIN 
	                                            sys.foreign_keys fk ON fk.object_id=fkc.constraint_object_id  JOIN
                                                sys.columns c1 ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id JOIN
                                                sys.columns c2 ON fkc.referenced_column_id = c2.column_id AND fkc.referenced_object_id = c2.object_id
                                            WHERE
	                                            fk.name=@fkname", conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        cmd.Parameters.Add("@fkname", SqlDbType.VarChar, 255).Value = fkName;
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt.AsEnumerable().Select(dr => new string[]
                        {
                            dr[0].ToString(),
                            dr[1].ToString(),
                            dr[2].ToString(),
                            dr[3].ToString()
                        }).ToArray();
                    }
                }
            }
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

        private string GetDirectoryPath()
        {
            var directoryName = _configuration.GetValue<string>("SQLQuery:savedSQLDirectoryName");
            var directoryPath = Path.Combine(_environment.ContentRootPath, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        private List<object[]> ConvertToArray(DataTable dataTable)
        {
            var result = new List<object[]>(dataTable.Rows.Count);
            for (int p = 0; p < dataTable.Rows.Count; p++)
            {
                result.Add(dataTable.Rows[p].ItemArray);
            }
            return result;
        }

        private DataRow FillDataRow(DataTable table, object[] row)
        {
            DataRow dataRow = table.NewRow();
            for (int i = 0; i < row.Length; i++)
            {
                try
                {
                    dataRow[i] = row[i] == null ? System.DBNull.Value : ((JsonElement)(row[i])).Deserialize(table.Columns[i].DataType);
                }
                catch 
                {
                    dataRow[i] = System.DBNull.Value;
                }
            }
            return dataRow;
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
