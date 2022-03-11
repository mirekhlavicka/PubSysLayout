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
    public class LayoutAssignsController : ControllerBase
    {
        private readonly LayoutDBContext _context;

        public LayoutAssignsController(LayoutDBContext context)
        {
            _context = context;
        }

        // GET: api/LayoutAssigns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LayoutAssign>>> GetLayoutAssigns(int? id_qslayout, int? id_layoutdefinition)
        {
            var res = _context.LayoutAssigns.AsQueryable();

            if (id_qslayout.HasValue && id_qslayout != 0)
            {
                res = res.Where(la => la.IdQslayout == id_qslayout);
            }

            if (id_layoutdefinition.HasValue && id_layoutdefinition != 0)
            {
                res = res.Where(la => la.IdLayoutdefinition == id_layoutdefinition);
            }

            return await res.ToListAsync();
        }

        // GET: api/LayoutAssigns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LayoutAssign>> GetLayoutAssign(int id)
        {
            var layoutAssign = await _context.LayoutAssigns.FindAsync(id);

            if (layoutAssign == null)
            {
                return NotFound();
            }

            return layoutAssign;
        }

        // PUT: api/LayoutAssigns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLayoutAssign(int id, LayoutAssign layoutAssign)
        {
            if (id != layoutAssign.IdServer)
            {
                return BadRequest();
            }

            _context.Entry(layoutAssign).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayoutAssignExists(id))
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

        // POST: api/LayoutAssigns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LayoutAssign>> PostLayoutAssign(LayoutAssign layoutAssign)
        {
            _context.LayoutAssigns.Add(layoutAssign);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LayoutAssignExists(layoutAssign.IdServer))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLayoutAssign", new { id = layoutAssign.IdServer }, layoutAssign);
        }

        // DELETE: api/LayoutAssigns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLayoutAssign(int id)
        {
            var layoutAssign = await _context.LayoutAssigns.FindAsync(id);
            if (layoutAssign == null)
            {
                return NotFound();
            }

            _context.LayoutAssigns.Remove(layoutAssign);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LayoutAssignExists(int id)
        {
            return _context.LayoutAssigns.Any(e => e.IdServer == id);
        }
    }
}
