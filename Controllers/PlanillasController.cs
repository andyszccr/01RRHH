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
    public class PlanillasController : Controller
    {
        private readonly DbrrhhContext _context;

        public PlanillasController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Planillas
        public async Task<IActionResult> Index()
        {
            var dbrrhhContext = _context.Planillas.Include(p => p.ImpuestoRentaNavigation).Include(p => p.TipoDeduccion).Include(p => p.Usuario);
            return View(await dbrrhhContext.ToListAsync());
        }

        // GET: Planillas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planilla = await _context.Planillas
                .Include(p => p.ImpuestoRentaNavigation)
                .Include(p => p.TipoDeduccion)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.PlanillaId == id);
            if (planilla == null)
            {
                return NotFound();
            }

            return View(planilla);
        }

        // GET: Planillas/Create
        public IActionResult Create()
        {
            ViewData["ImpuestoRentaId"] = new SelectList(_context.ImpuestoRenta, "Porcentaje", "Porcentaje");
            ViewData["TipoDeduccionId"] = new SelectList(_context.TipoDeducciones, "DeduccionNombre", "DeduccionNombre");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            return View();
        }

        // POST: Planillas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanillaId,FechaPlanilla,UsuarioId,HorasTrabajadas,HorasExtras,TipoDeduccionId,Deducciones,ImpuestoRentaId,ImpuestoRenta,SalarioNeto,PlanillaFechaPago,PlanillaStatus,Banco,NumeroCuenta")] Planilla planilla)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planilla);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ImpuestoRentaId"] = new SelectList(_context.ImpuestoRenta, "ImpuestoRentaId", "ImpuestoRentaId", planilla.ImpuestoRentaId);
            ViewData["TipoDeduccionId"] = new SelectList(_context.TipoDeducciones, "TipoDeduccionId", "TipoDeduccionId", planilla.TipoDeduccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", planilla.UsuarioId);
            return View(planilla);
        }

        // GET: Planillas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planilla = await _context.Planillas.FindAsync(id);
            if (planilla == null)
            {
                return NotFound();
            }
            ViewData["ImpuestoRentaId"] = new SelectList(_context.ImpuestoRenta, "ImpuestoRentaId", "ImpuestoRentaId", planilla.ImpuestoRentaId);
            ViewData["TipoDeduccionId"] = new SelectList(_context.TipoDeducciones, "TipoDeduccionId", "TipoDeduccionId", planilla.TipoDeduccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", planilla.UsuarioId);
            return View(planilla);
        }

        // POST: Planillas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlanillaId,FechaPlanilla,UsuarioId,HorasTrabajadas,HorasExtras,TipoDeduccionId,Deducciones,ImpuestoRentaId,ImpuestoRenta,SalarioNeto,PlanillaFechaPago,PlanillaStatus,Banco,NumeroCuenta")] Planilla planilla)
        {
            if (id != planilla.PlanillaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planilla);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanillaExists(planilla.PlanillaId))
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
            ViewData["ImpuestoRentaId"] = new SelectList(_context.ImpuestoRenta, "ImpuestoRentaId", "ImpuestoRentaId", planilla.ImpuestoRentaId);
            ViewData["TipoDeduccionId"] = new SelectList(_context.TipoDeducciones, "TipoDeduccionId", "TipoDeduccionId", planilla.TipoDeduccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", planilla.UsuarioId);
            return View(planilla);
        }

        // GET: Planillas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planilla = await _context.Planillas
                .Include(p => p.ImpuestoRentaNavigation)
                .Include(p => p.TipoDeduccion)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.PlanillaId == id);
            if (planilla == null)
            {
                return NotFound();
            }

            return View(planilla);
        }

        // POST: Planillas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planilla = await _context.Planillas.FindAsync(id);
            if (planilla != null)
            {
                _context.Planillas.Remove(planilla);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanillaExists(int id)
        {
            return _context.Planillas.Any(e => e.PlanillaId == id);
        }
    }
}
