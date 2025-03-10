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
    public class TipoDeduccionesController : Controller
    {
        private readonly DbrrhhContext _context;

        public TipoDeduccionesController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: TipoDeducciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoDeducciones.ToListAsync());
        }

        // GET: TipoDeducciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDeduccione = await _context.TipoDeducciones
                .FirstOrDefaultAsync(m => m.TipoDeduccionId == id);
            if (tipoDeduccione == null)
            {
                return NotFound();
            }

            return View(tipoDeduccione);
        }

        // GET: TipoDeducciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoDeducciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoDeduccionId,DeduccionNombre,DeduccionPorcentaje")] TipoDeduccione tipoDeduccione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoDeduccione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDeduccione);
        }

        // GET: TipoDeducciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDeduccione = await _context.TipoDeducciones.FindAsync(id);
            if (tipoDeduccione == null)
            {
                return NotFound();
            }
            return View(tipoDeduccione);
        }

        // POST: TipoDeducciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoDeduccionId,DeduccionNombre,DeduccionPorcentaje")] TipoDeduccione tipoDeduccione)
        {
            if (id != tipoDeduccione.TipoDeduccionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoDeduccione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDeduccioneExists(tipoDeduccione.TipoDeduccionId))
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
            return View(tipoDeduccione);
        }

        // GET: TipoDeducciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDeduccione = await _context.TipoDeducciones
                .FirstOrDefaultAsync(m => m.TipoDeduccionId == id);
            if (tipoDeduccione == null)
            {
                return NotFound();
            }

            return View(tipoDeduccione);
        }

        // POST: TipoDeducciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoDeduccione = await _context.TipoDeducciones.FindAsync(id);
            if (tipoDeduccione != null)
            {
                _context.TipoDeducciones.Remove(tipoDeduccione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoDeduccioneExists(int id)
        {
            return _context.TipoDeducciones.Any(e => e.TipoDeduccionId == id);
        }
    }
}
