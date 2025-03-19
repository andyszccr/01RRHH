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
    public class PermisoesController : Controller
    {
        private readonly DbrrhhContext _context;

        public PermisoesController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Permisoes
        public async Task<IActionResult> Index()
        {
            var dbrrhhContext = _context.Permisos.Include(p => p.TipoPermiso).Include(p => p.Usuario).Include(p => p.UsuarioIdaprobadoPorNavigation);
            return View(await dbrrhhContext.ToListAsync());
        }

        // GET: Permisoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .Include(p => p.TipoPermiso)
                .Include(p => p.Usuario)
                .Include(p => p.UsuarioIdaprobadoPorNavigation)
                .FirstOrDefaultAsync(m => m.PermisoId == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // GET: Permisoes/Create
        public IActionResult Create()
        {
            ViewData["TipoPermisoId"] = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermisoId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            ViewData["UsuarioIdaprobadoPor"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            return View();
        }

        // POST: Permisoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PermisoId,UsuarioId,TipoPermisoId,PermisoStatus,HorasSolicitadas,PermisoCreacion,PermisoUpdate,PermisoDelete,UsuarioIdaprobadoPor,Motivo")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoPermisoId"] = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermisoId", permiso.TipoPermisoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioId);
            ViewData["UsuarioIdaprobadoPor"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioIdaprobadoPor);
            return View(permiso);
        }

        // GET: Permisoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }
            ViewData["TipoPermisoId"] = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermisoId", permiso.TipoPermisoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioId);
            ViewData["UsuarioIdaprobadoPor"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioIdaprobadoPor);
            return View(permiso);
        }

        // POST: Permisoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PermisoId,UsuarioId,TipoPermisoId,PermisoStatus,HorasSolicitadas,PermisoCreacion,PermisoUpdate,PermisoDelete,UsuarioIdaprobadoPor,Motivo")] Permiso permiso)
        {
            if (id != permiso.PermisoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisoExists(permiso.PermisoId))
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
            ViewData["TipoPermisoId"] = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermisoId", permiso.TipoPermisoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioId);
            ViewData["UsuarioIdaprobadoPor"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioIdaprobadoPor);
            return View(permiso);
        }

        // GET: Permisoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .Include(p => p.TipoPermiso)
                .Include(p => p.Usuario)
                .Include(p => p.UsuarioIdaprobadoPorNavigation)
                .FirstOrDefaultAsync(m => m.PermisoId == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // POST: Permisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso != null)
            {
                _context.Permisos.Remove(permiso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermisoExists(int id)
        {
            return _context.Permisos.Any(e => e.PermisoId == id);
        }
    }
}
