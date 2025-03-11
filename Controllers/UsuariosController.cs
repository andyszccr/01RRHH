using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;

namespace RRHH.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbrrhhContext _context;

        public UsuariosController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Departamento)
                .Include(u => u.Provincia)
                .Include(u => u.Canton)
                .Include(u => u.Distrito)
                .ToListAsync();
            return View(usuarios);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            await LoadViewBags();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(usuario);
            }

            usuario.UsuarioStatus = true;
            usuario.UsuarioCreacion = DateTime.Now;
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            await LoadViewBags();
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Usuarios.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Solo actualiza la contraseña si se proporcionó una nueva
                    usuario.Contrasena = string.IsNullOrEmpty(usuario.Contrasena) ? existingUser.Contrasena : usuario.Contrasena;

                    usuario.UsuarioUpdate = DateTime.Now;
                    _context.Entry(existingUser).CurrentValues.SetValues(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        return NotFound();
                    }
                    throw; // Re-lanzar la excepción si ocurre un error
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadViewBags();
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.UsuarioDelete = DateTime.Now;
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id) => _context.Usuarios.Any(e => e.UsuarioId == id);

        // Cargar listas para los ViewBags
        private async Task LoadViewBags()
        {
            ViewBag.RolList = await _context.Roles.ToListAsync();
            ViewBag.DepartamentoList = await _context.Departamentos.ToListAsync();
            ViewBag.ProvinciaList = await _context.Provincia.ToListAsync();
            ViewBag.CantonList = await _context.Cantons.ToListAsync();
            ViewBag.DistritoList = await _context.Distritos.ToListAsync();
        }

        // GET: Usuarios/GetCantones/5
        public async Task<JsonResult> GetCantones(int provinciaId)
        {
            var cantones = await _context.Cantons
                .Where(c => c.ProvinciaId == provinciaId)
                .Select(c => new { c.CantonId, c.Canton1 })
                .ToListAsync();
            return Json(cantones);
        }

        // GET: Usuarios/GetDistritos/5
        public async Task<JsonResult> GetDistritos(int cantonId)
        {
            var distritos = await _context.Distritos
                .Where(d => d.CantonId == cantonId)
                .Select(d => new { d.DistritoId, d.Distrito1 })
                .ToListAsync();
            return Json(distritos);
        }
    }
}