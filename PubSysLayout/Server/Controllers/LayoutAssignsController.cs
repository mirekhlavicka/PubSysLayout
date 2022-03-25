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

        // GET: api/LayoutAssigns/5/6/7
        [HttpGet("{id_server}/{id_section}/{id_qslayout}")]
        public async Task<ActionResult<LayoutAssign>> GetLayoutAssign(int id_server, int id_section, int id_qslayout)
        {
            var layoutAssign = await _context.LayoutAssigns.SingleOrDefaultAsync(la => la.IdServer == id_server && la.IdSection == id_section && la.IdQslayout == id_qslayout);

            if (layoutAssign == null)
            {
                return NotFound();
            }

            return layoutAssign;
        }

        // PUT: api/LayoutAssigns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id_server}/{id_section}/{id_qslayout}")]
        public async Task<IActionResult> PutLayoutAssign(int id_server, int id_section, int id_qslayout, LayoutAssign layoutAssign, bool force)
        {
            if (id_server != layoutAssign.IdServer || id_section != layoutAssign.IdSection || id_qslayout != layoutAssign.IdQslayout)
            {
                var la = _context.LayoutAssigns.Include(laa => laa.IdLayoutdefinitionNavigation).SingleOrDefault(laa => laa.IdServer == layoutAssign.IdServer && laa.IdSection == layoutAssign.IdSection && laa.IdQslayout == layoutAssign.IdQslayout);

                if (la != null)
                {
                    if (force)
                    {
                        _context.LayoutAssigns.Remove(la);
                    }
                    else
                    {
                        return Conflict(la.IdLayoutdefinitionNavigation.Name);
                    }
                }

                la = _context.LayoutAssigns.SingleOrDefault(laa => laa.IdServer == id_server && laa.IdSection == id_section && laa.IdQslayout == id_qslayout);

                _context.LayoutAssigns.Remove(la);

                _context.LayoutAssigns.Add(layoutAssign);
            }
            else
            {
                _context.Entry(layoutAssign).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayoutAssignExists(id_server, id_section, id_qslayout))
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
        public async Task<ActionResult<LayoutAssign>> PostLayoutAssign(LayoutAssign layoutAssign, bool force)
        {
            var la = _context.LayoutAssigns.Include(laa => laa.IdLayoutdefinitionNavigation).SingleOrDefault(laa => laa.IdServer == layoutAssign.IdServer && laa.IdSection == layoutAssign.IdSection && laa.IdQslayout == layoutAssign.IdQslayout);

            if (la != null)
            {
                if (force)
                {
                    _context.LayoutAssigns.Remove(la);
                }
                else
                {
                    return Conflict(la.IdLayoutdefinitionNavigation.Name);
                }
            }

            _context.LayoutAssigns.Add(layoutAssign);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LayoutAssignExists(layoutAssign.IdServer, layoutAssign.IdSection, layoutAssign.IdQslayout))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLayoutAssign", new { id_server = layoutAssign.IdServer, id_section = layoutAssign.IdSection, id_qslayout = layoutAssign.IdQslayout}, layoutAssign);
        }

        // DELETE: api/LayoutAssigns
        [HttpDelete("{id_server}/{id_section}/{id_qslayout}")]
        public async Task<IActionResult> DeleteLayoutAssign(int id_server, int id_section, int id_qslayout)
        {
            var layoutAssign = await _context.LayoutAssigns.SingleOrDefaultAsync(la => la.IdServer == id_server && la.IdSection == id_section && la.IdQslayout == id_qslayout);
            if (layoutAssign == null)
            {
                return NotFound();
            }

            _context.LayoutAssigns.Remove(layoutAssign);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LayoutAssignExists(int id_server, int id_section, int id_qslayout)
        {
            return _context.LayoutAssigns.Any(e => e.IdServer == id_server && e.IdSection == id_section && e.IdQslayout == id_qslayout);
        }

        [HttpGet("layoutdefinitions")]
        public async Task<ActionResult<int[]>> GetLayoutDefinitions()
        {
            var res = await _context.LayoutAssigns.Select(la => la.IdLayoutdefinition).Distinct().ToArrayAsync();

            return res;
        }

        [HttpGet("qslayouts")]
        public async Task<ActionResult<int[]>> GetQSLayous()
        {
            var res = await _context.LayoutAssigns.Select(la => la.IdQslayout).Distinct().ToArrayAsync();

            return res;
        }
    }
}
