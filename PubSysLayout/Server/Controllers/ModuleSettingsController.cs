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
    public class ModuleSettingsController : ControllerBase
    {
        private readonly LayoutDBContext _context;

        public ModuleSettingsController(LayoutDBContext context)
        {
            _context = context;
        }

        // GET: api/ModuleSettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleSetting>>> GetModuleSettings(int id_module, int? id_moduleusage)
        {
            var defaultSettings = await _context.ModuleSettings.Where(ms => ms.IdModule == id_module && ms.IdModuleusage == 0).ToListAsync();

            if ((id_moduleusage??0) == 0)
            { 
                return defaultSettings.OrderBy(ms => ms.SettingName).ToList();
            }
            else
            {
                var settings = await _context.ModuleSettings.Where(ms => ms.IdModule == id_module && ms.IdModuleusage == id_moduleusage.Value).ToDictionaryAsync(ms => ms.SettingName, ms => ms);

                return defaultSettings.Where( ds => !settings.ContainsKey(ds.SettingName)).Concat(settings.Values).OrderBy(ms => ms.SettingName).ToList();
            }
        }

        // GET: api/ModuleSettings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleSetting>> GetModuleSetting(int id)
        {
            var moduleSetting = await _context.ModuleSettings.FindAsync(id);

            if (moduleSetting == null)
            {
                return NotFound();
            }

            return moduleSetting;
        }

        // PUT: api/ModuleSettings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModuleSetting(int id, ModuleSetting moduleSetting)
        {
            if (id != moduleSetting.IdSetting)
            {
                return BadRequest();
            }

            _context.Entry(moduleSetting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleSettingExists(id))
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

        // POST: api/ModuleSettings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ModuleSetting>> PostModuleSetting(ModuleSetting moduleSetting)
        {
            _context.ModuleSettings.Add(moduleSetting);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModuleSetting", new { id = moduleSetting.IdSetting }, moduleSetting);
        }

        // DELETE: api/ModuleSettings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModuleSetting(int id)
        {
            var moduleSetting = await _context.ModuleSettings.FindAsync(id);
            if (moduleSetting == null)
            {
                return NotFound();
            }

            _context.ModuleSettings.Remove(moduleSetting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModuleSettingExists(int id)
        {
            return _context.ModuleSettings.Any(e => e.IdSetting == id);
        }
    }
}
