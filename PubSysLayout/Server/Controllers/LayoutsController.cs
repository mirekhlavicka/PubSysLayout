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
    public class LayoutsController : ControllerBase
    {
        private readonly LayoutDBContext _context;

        public LayoutsController(LayoutDBContext context)
        {
            _context = context;
        }

        // GET: api/Layouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Layout>>> GetLayouts()
        {
            return await _context.Layouts.ToListAsync();
        }

        // GET: api/Layouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Layout>> GetLayout(int id)
        {
            var layout = await _context.Layouts.FindAsync(id);

            if (layout == null)
            {
                return NotFound();
            }

            return layout;
        }

        // PUT: api/Layouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLayout(int id, Layout layout)
        {
            if (id != layout.IdLayout)
            {
                return BadRequest();
            }

            _context.Entry(layout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayoutExists(id))
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

        // POST: api/Layouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Layout>> PostLayout(Layout layout)
        {
            _context.Layouts.Add(layout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLayout", new { id = layout.IdLayout }, layout);
        }

        // DELETE: api/Layouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLayout(int id)
        {
            var layout = await _context.Layouts.FindAsync(id);
            if (layout == null)
            {
                return NotFound();
            }

            _context.Layouts.Remove(layout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LayoutExists(int id)
        {
            return _context.Layouts.Any(e => e.IdLayout == id);
        }
    }
}
