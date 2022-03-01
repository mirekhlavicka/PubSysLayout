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
        public async Task<ActionResult<IEnumerable<LayoutDefinition>>> GetLayoutDefinitions()
        {
            return await _context.LayoutDefinitions.ToListAsync();
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

            //foreach (var mu in layoutDefinition.ModuleUsages)
            //{
            //    _context.Entry(mu).State = EntityState.Modified;
            //}

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

        private bool LayoutDefinitionExists(int id)
        {
            return _context.LayoutDefinitions.Any(e => e.IdLayoutdefinition == id);
        }
    }
}
