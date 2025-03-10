 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;

namespace RRHH.Controllers
{
    public class HorasExtrasController : Controller
    {
        private readonly DbrrhhContext _context;

        public HorasExtrasController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: HorasExtras
        public async Task<IActionResult> Index()
        {
            var dbrrhhContext = _context.HorasExtras.Include(h => h.TipoJornada).Include(h => h.Usuario);
            return View(await dbrrhhContext.ToListAsync());
        }

        // GET: HorasExtras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horasExtra = await _context.HorasExtras
                .Include(h => h.TipoJornada)
                .Include(h => h.Usuario)
                .FirstOrDefaultAsync(m => m.HorasExtraId == id);
            if (horasExtra == null)
            {
                return NotFound();
            }

            return View(horasExtra);
        }

        // GET: HorasExtras/Create
        public IActionResult Create()
        {
            ViewData["TipoJornadaId"] = new SelectList(_context.TipoJornada, "TipoJornadaId", "TipoJornadaId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: HorasExtras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HorasExtraId,UsuarioId,TipoJornadaId,Fecha,TipoPago,HorasExtra1,MontoPagoSalario,Motivo")] HorasExtra horasExtra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(horasExtra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoJornadaId"] = new SelectList(_context.TipoJornada, "TipoJornadaId", "TipoJornadaId", horasExtra.TipoJornadaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", horasExtra.UsuarioId);
            return View(horasExtra);
        }

        // GET: HorasExtras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horasExtra = await _context.HorasExtras.FindAsync(id);
            if (horasExtra == null)
            {
                return NotFound();
            }
            ViewData["TipoJornadaId"] = new SelectList(_context.TipoJornada, "TipoJornadaId", "TipoJornadaId", horasExtra.TipoJornadaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", horasExtra.UsuarioId);
            return View(horasExtra);
        }

        // POST: HorasExtras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HorasExtraId,UsuarioId,TipoJornadaId,Fecha,TipoPago,HorasExtra1,MontoPagoSalario,Motivo")] HorasExtra horasExtra)
        {
            if (id != horasExtra.HorasExtraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(horasExtra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HorasExtraExists(horasExtra.HorasExtraId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoJornadaId"] = new SelectList(_context.TipoJornada, "TipoJornadaId", "TipoJornadaId", horasExtra.TipoJornadaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", horasExtra.UsuarioId);
            return View(horasExtra);
        }

        // GET: HorasExtras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horasExtra = await _context.HorasExtras
                .Include(h => h.TipoJornada)
                .Include(h => h.Usuario)
                .FirstOrDefaultAsync(m => m.HorasExtraId == id);
            if (horasExtra == null)
            {
                return NotFound();
            }

            return View(horasExtra);
        }

        // POST: HorasExtras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var horasExtra = await _context.HorasExtras.FindAsync(id);
            if (horasExtra != null)
            {
                _context.HorasExtras.Remove(horasExtra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HorasExtraExists(int id)
        {
            return _context.HorasExtras.Any(e => e.HorasExtraId == id);
        }
    }
}
