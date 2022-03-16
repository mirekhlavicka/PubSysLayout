using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubSysLayout.Shared.Model;

namespace PubSysLayout.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DBListController : ControllerBase
    {
        private readonly LayoutDBContext _context;
        private readonly IConfiguration _configuration;

        public DBListController(LayoutDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<string[]>> GetConList()
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = _configuration["SELECT_databases"];

                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    var entities = new List<string>();

                    while (result.Read())
                    {
                        entities.Add(result[0].ToString());
                    }
                    _context.Database.CloseConnection();

                    return entities.ToArray();
                }
            }
        }

        [HttpGet("server")]
        public async Task<ActionResult<string>> GetServerName()
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT name FROM Servers WHERE id_server=1";
                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();

                return (string)await command.ExecuteScalarAsync();
            }
        }

        // GET: api/dblist/servers
        [HttpGet("servers")]
        public async Task<ActionResult<IEnumerable<PubSysLayout.Shared.Model.Server>>> GetServers()
        {
            return await _context.Servers.FromSqlRaw(@"
                SELECT 
                    *
                FROM 
                    servers
                WHERE
                    del = 0
                ").ToListAsync();
        }

        // GET: api/dblist/section/5
        [HttpGet("section/{id}")]
        public async Task<ActionResult<Section>> GetSection(int id)
        {
            return await _context.Sections.FromSqlRaw(@$"
                SELECT 
                    id_section, id_metasection, id_server, id_section_parent, id_section_parent_top, treelevel, name, redirurl, target, visible, del, [order], options,
                    0 AS id_file, 0 AS tag 
                FROM 
                    sections s
                WHERE
                   id_section = {id}
                ").FirstOrDefaultAsync();
        }

        // GET: api/dblist/sections
        [HttpGet("sections")]
        public async Task<ActionResult<IEnumerable<Section>>> GetSections(string search)
        {
            if (search == null)
            {
                return await _context.Sections.FromSqlRaw(@"
                SELECT 
                    id_section, id_metasection, id_server, id_section_parent, id_section_parent_top, treelevel, name, redirurl, target, visible, del, [order], options,
                    0 AS id_file, 0 AS tag 
                FROM 
                    sections s
                --WHERE
                --    del = 0
                ORDER BY
                    name
                ").ToListAsync();
            }

            return await _context.Sections.FromSqlRaw(@$"
                SELECT 
                    id_section, id_metasection, id_server, id_section_parent, id_section_parent_top, treelevel, name, redirurl, target, visible, del, [order], options,
                    0 AS id_file, 0 AS tag 
                FROM 
                    sections s
                WHERE
                   name LIKE '%{search}%' OR CAST(id_section AS VARCHAR(10))={search}
                ORDER BY
                    name
                ").ToListAsync();
        }
    }
}
