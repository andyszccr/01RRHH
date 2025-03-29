using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;
using System.Threading.Tasks;

namespace RRHH.Controllers
{
    public class CuentaUsuarioController : Controller
    {
        private readonly DbrrhhContext _context;

        public CuentaUsuarioController(DbrrhhContext context)
        {
            _context = context;
        }

        // Acción para cerrar sesión
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Login
        public IActionResult Login()
        {
            // Si ya hay una sesión activa, redirigir al inicio
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Usuario model)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.NombreUsuario == model.NombreUsuario && u.Contrasena == model.Contrasena);

            if (usuario != null)
            {
                // Guardar información en la sesión
                HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
                HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
                HttpContext.Session.SetString("RolId", usuario.RolId.ToString());
                
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // GET: Register
        public async Task<IActionResult> Register()
        {
            await LoadViewBags();
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.UsuarioCreacion = DateTime.Now;
                usuario.UsuarioStatus = true;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            await LoadViewBags();
            return View(usuario);
        }

        // GET: Perfil
        public async Task<IActionResult> Perfil()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
            {
                return RedirectToAction("Login");
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Departamento)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId.ToString() == usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            await LoadViewBags();
            return View(usuario);
        }

        // POST: Perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.UsuarioUpdate = DateTime.Now;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Perfil));
            }
            await LoadViewBags();
            return View(usuario);
        }

        private async Task LoadViewBags()
        {
            ViewBag.RolList = await _context.Roles.ToListAsync();
            ViewBag.DepartamentoList = await _context.Departamentos.ToListAsync();
            ViewBag.ProvinciaList = await _context.Provincia.ToListAsync();
            ViewBag.CantonList = await _context.Cantons.ToListAsync();
            ViewBag.DistritoList = await _context.Distritos.ToListAsync();
        }
    }
}