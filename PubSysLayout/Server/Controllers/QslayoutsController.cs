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
    public class QslayoutsController : ControllerBase
    {
        private readonly LayoutDBContext _context;

        public QslayoutsController(LayoutDBContext context)
        {
            _context = context;
        }

        // GET: api/Qslayouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Qslayout>>> GetQslayouts()
        {
            return await _context.Qslayouts.ToListAsync();
        }

        // GET: api/Qslayouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Qslayout>> GetQslayout(int id)
        {
            var qslayout = await _context.Qslayouts.FindAsync(id);

            if (qslayout == null)
            {
                return NotFound();
            }

            return qslayout;
        }

        // PUT: api/Qslayouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQslayout(int id, Qslayout qslayout)
        {
            if (id != qslayout.IdQslayout)
            {
                return BadRequest();
            }

            _context.Entry(qslayout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QslayoutExists(id))
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

        // POST: api/Qslayouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Qslayout>> PostQslayout(Qslayout qslayout)
        {
            _context.Qslayouts.Add(qslayout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQslayout", new { id = qslayout.IdQslayout }, qslayout);
        }

        // DELETE: api/Qslayouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQslayout(int id)
        {
            var qslayout = await _context.Qslayouts.FindAsync(id);
            if (qslayout == null)
            {
                return NotFound();
            }

            _context.Qslayouts.Remove(qslayout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QslayoutExists(int id)
        {
            return _context.Qslayouts.Any(e => e.IdQslayout == id);
        }
    }
}
