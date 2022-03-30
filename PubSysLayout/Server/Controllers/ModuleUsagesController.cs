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
    public class ModuleUsagesController : ControllerBase
    {
        private readonly LayoutDBContext _context;

        public ModuleUsagesController(LayoutDBContext context)
        {
            _context = context;
        }

        // GET: api/ModuleUsages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleUsage>>> GetModuleUsages(int? id_module)
        {
            var res = _context.ModuleUsages.AsQueryable();

            if (id_module.HasValue && id_module != 0)
            {
                res = res.Where(mu => mu.IdModule == id_module);
            }

            return await res.Include(mu => mu.IdSpotNavigation).Include(mu => mu.IdLayoutdefinitionNavigation).ToListAsync();
        }

        // GET: api/ModuleUsages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleUsage>> GetModuleUsage(int id)
        {
            var moduleUsage = await _context.ModuleUsages.FindAsync(id);

            if (moduleUsage == null)
            {
                return NotFound();
            }

            return moduleUsage;
        }

        // PUT: api/ModuleUsages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModuleUsage(int id, ModuleUsage moduleUsage)
        {
            if (id != moduleUsage.IdModuleusage)
            {
                return BadRequest();
            }

            _context.Entry(moduleUsage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleUsageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ModuleUsages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ModuleUsage>> PostModuleUsage(ModuleUsage moduleUsage)
        {
            _context.ModuleUsages.Add(moduleUsage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModuleUsage", new { id = moduleUsage.IdModuleusage }, moduleUsage);
        }

        // DELETE: api/ModuleUsages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModuleUsage(int id)
        {
            var moduleUsage = await _context.ModuleUsages.FindAsync(id);
            if (moduleUsage == null)
            {
                return NotFound();
            }

            _context.ModuleUsages.Remove(moduleUsage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("modules")]
        public async Task<ActionResult<int[]>> GetModules()
        {
            var res = await _context.ModuleUsages.Select(mu => mu.IdModule).Distinct().ToArrayAsync();

            return res;
        }

        [HttpGet("spots")]
        public async Task<ActionResult<int[]>> GetSpots()
        {
            var res = await _context.ModuleUsages.Select(mu => mu.IdSpot).Distinct().ToArrayAsync();

            return res;
        }

        private bool ModuleUsageExists(int id)
        {
            return _context.ModuleUsages.Any(e => e.IdModuleusage == id);
        }
    }
}
