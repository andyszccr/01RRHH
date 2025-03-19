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
    public class VacacionesController : Controller
    {
        private readonly DbrrhhContext _context;

        public VacacionesController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Vacaciones
        public async Task<IActionResult> Index()
        {
            var dbrrhhContext = _context.Vacaciones.Include(v => v.Usuario);
            return View(await dbrrhhContext.ToListAsync());
        }

        // GET: Vacaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacione = await _context.Vacaciones
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(m => m.VacacionId == id);
            if (vacacione == null)
            {
                return NotFound();
            }

            return View(vacacione);
        }

        // GET: Vacaciones/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            return View();
        }

        // POST: Vacaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VacacionId,UsuarioId,FechaSolicitud,FechaInicio,FechaFin,DiasSolicitados,DiasAprobados,Estado,Comentarios,FechaAprobacion,FechaCancelacion,SalarioVacaciones")] Vacacione vacacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vacacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", vacacione.UsuarioId);
            return View(vacacione);
        }

        // GET: Vacaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacione = await _context.Vacaciones.FindAsync(id);
            if (vacacione == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", vacacione.UsuarioId);
            return View(vacacione);
        }

        // POST: Vacaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VacacionId,UsuarioId,FechaSolicitud,FechaInicio,FechaFin,DiasSolicitados,DiasAprobados,Estado,Comentarios,FechaAprobacion,FechaCancelacion,SalarioVacaciones")] Vacacione vacacione)
        {
            if (id != vacacione.VacacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacacioneExists(vacacione.VacacionId))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", vacacione.UsuarioId);
            return View(vacacione);
        }

        // GET: Vacaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacione = await _context.Vacaciones
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(m => m.VacacionId == id);
            if (vacacione == null)
            {
                return NotFound();
            }

            return View(vacacione);
        }

        // POST: Vacaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacacione = await _context.Vacaciones.FindAsync(id);
            if (vacacione != null)
            {
                _context.Vacaciones.Remove(vacacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacacioneExists(int id)
        {
            return _context.Vacaciones.Any(e => e.VacacionId == id);
        }
    }
}
