﻿using System;
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
    }
}