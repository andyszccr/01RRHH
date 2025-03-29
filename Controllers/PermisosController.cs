using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;
using RRHH.Filters;

namespace RRHH.Controllers
{
    [ServiceFilter(typeof(AutorizacionFilter))]
    public class PermisosController : Controller
    {
        private readonly DbrrhhContext _context;

        public PermisosController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Permisos
        public async Task<IActionResult> Index()
        {
            var dbrrhhContext = _context.Permisos
                .Include(p => p.TipoPermiso)
                .Include(p => p.Usuario)
                .Include(p => p.UsuarioIdaprobadoPorNavigation)
                .OrderByDescending(p => p.PermisoCreacion);
            return View(await dbrrhhContext.ToListAsync());
        }

        // GET: Permisos/Details/5
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

        // GET: Permisos/Create
        public IActionResult Create()
        {
            ViewBag.TipoPermisoId = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermiso1");
            ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            ViewBag.UsuarioIdaprobadoPor = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            return View();
        }

        // POST: Permisos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,TipoPermisoId,HorasSolicitadas,Motivo")] Permiso permiso)
        {
            try
            {
                // Validar que los IDs existan
                if (!await _context.Usuarios.AnyAsync(u => u.UsuarioId == permiso.UsuarioId))
                {
                    ModelState.AddModelError("UsuarioId", "El usuario seleccionado no existe");
                }

                if (!await _context.TipoPermisos.AnyAsync(t => t.TipoPermisoId == permiso.TipoPermisoId))
                {
                    ModelState.AddModelError("TipoPermisoId", "El tipo de permiso seleccionado no existe");
                }


                // Establecer valores por defecto
                permiso.PermisoCreacion = DateTime.Now;
                permiso.PermisoStatus = 2; // Pendiente por defecto
                permiso.UsuarioIdaprobadoPor = null; // Inicialmente no tiene aprobador
                
                _context.Add(permiso);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Permiso creado exitosamente.";


                // Si hay error de validación, recargar los ViewBags
                ViewBag.TipoPermisoId = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermiso1", permiso.TipoPermisoId);
                ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el permiso: " + ex.Message);
                ViewBag.TipoPermisoId = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermiso1");
                ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
                return View(permiso);
            }
        }

        // GET: Permisos/Edit/5
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
            ViewBag.TipoPermisoId = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermiso1", permiso.TipoPermisoId);
            ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioId);
            ViewBag.UsuarioIdaprobadoPor = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioIdaprobadoPor);
            return View(permiso);
        }

        // POST: Permisos/Edit/5
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
                    permiso.PermisoUpdate = DateTime.Now;
                    _context.Update(permiso);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Permiso actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
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
            }
            ViewBag.TipoPermisoId = new SelectList(_context.TipoPermisos, "TipoPermisoId", "TipoPermiso1", permiso.TipoPermisoId);
            ViewBag.UsuarioId = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioId);
            ViewBag.UsuarioIdaprobadoPor = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", permiso.UsuarioIdaprobadoPor);
            return View(permiso);
        }

        // GET: Permisos/Delete/5
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

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso != null)
            {
                permiso.PermisoDelete = DateTime.Now;
                permiso.PermisoStatus = 3; // Rechazado
                _context.Update(permiso);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Permiso eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PermisoExists(int id)
        {
            return _context.Permisos.Any(e => e.PermisoId == id);
        }
    }
}
