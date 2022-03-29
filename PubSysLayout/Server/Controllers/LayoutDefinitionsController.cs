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
    public class LayoutDefinitionsController : ControllerBase
    {
        private readonly LayoutDBContext _context;

        public LayoutDefinitionsController(LayoutDBContext context)
        {
            _context = context;
        }

        // GET: api/LayoutDefinitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LayoutDefinition>>> GetLayoutDefinitions(bool? hideUnused)
        {
            var res = _context.LayoutDefinitions/*.Include(ld => ld.LayoutAssigns)*/.AsQueryable();

            if (hideUnused.HasValue && hideUnused.Value)
            {
                res = res.Where(m => m.LayoutAssigns.Any());
            }

            return await res.ToListAsync();
        }

        // GET: api/LayoutDefinitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LayoutDefinition>> GetLayoutDefinition(int id)
        {
            var layoutDefinition = await _context.LayoutDefinitions.Include(ld => ld.ModuleUsages).FirstOrDefaultAsync(ld => ld.IdLayoutdefinition == id);

            if (layoutDefinition == null)
            {
                return NotFound();
            }

            return layoutDefinition;
        }

        // PUT: api/LayoutDefinitions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLayoutDefinition(int id, LayoutDefinition layoutDefinition)
        {
            if (id != layoutDefinition.IdLayoutdefinition)
            {
                return BadRequest();
            }

            _context.Entry(layoutDefinition).State = EntityState.Modified;

            foreach (var mu in layoutDefinition.ModuleUsages) //!!!
            {
                _context.Entry(mu).State = EntityState.Unchanged; //!!!
            } //!!!

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayoutDefinitionExists(id))
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

        // POST: api/LayoutDefinitions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LayoutDefinition>> PostLayoutDefinition(LayoutDefinition layoutDefinition)
        {
            _context.LayoutDefinitions.Add(layoutDefinition);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLayoutDefinition", new { id = layoutDefinition.IdLayoutdefinition }, layoutDefinition);
        }

        // DELETE: api/LayoutDefinitions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLayoutDefinition(int id)
        {
            var layoutDefinition = await _context.LayoutDefinitions.FindAsync(id);
            if (layoutDefinition == null)
            {
                return NotFound();
            }

            _context.LayoutDefinitions.Remove(layoutDefinition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet()]
        [Route("copy/{id}")]
        public ActionResult<int> CopyLayoutDefinition(int id)
        {
            LayoutDefinition layoutDefinition = _context.LayoutDefinitions.Include(ld => ld.ModuleUsages).SingleOrDefault(ld => ld.IdLayoutdefinition == id);

            if (layoutDefinition == null)
            {
                return BadRequest();
            }

            LayoutDefinition nld = new LayoutDefinition
            {
                IdLayout = layoutDefinition.IdLayout,
                IdStyle = layoutDefinition.IdStyle,
                Mainstyle = layoutDefinition.Mainstyle,
                Name = "Copy of " + layoutDefinition.Name
            };

            foreach (ModuleUsage mu in layoutDefinition.ModuleUsages)
            {
                nld.ModuleUsages.Add(new ModuleUsage
                {
                    IdModule = mu.IdModule,
                    IdSpot = mu.IdSpot,
                    Order = mu.Order,
                    CacheTime = mu.CacheTime,
                    ShowMobile = mu.ShowMobile
                });
            }

            _context.LayoutDefinitions.Add(nld);

            _context.SaveChanges();

            return Ok(nld.IdLayoutdefinition);
        }


        private bool LayoutDefinitionExists(int id)
        {
            return _context.LayoutDefinitions.Any(e => e.IdLayoutdefinition == id);
        }

        [HttpGet("layouts")]
        public async Task<ActionResult<int[]>> GetLayouts()
        {
            var res = await _context.LayoutDefinitions.Select(ld => ld.IdLayout).Distinct().ToArrayAsync();

            return res;
        }
    }
}
