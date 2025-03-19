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

        // GET: Login
        public IActionResult Login()
        {
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
                // Aquí puedes iniciar sesión (ej., establecer cookie)
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // GET: Register
        public async Task<IActionResult> Register()
        {
            await LoadViewBags(); // Cargar listas para provincias, cantones y distritos
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario model)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = model.NombreUsuario,
                    Contrasena = model.Contrasena,
                    Cedula = model.Cedula,
                    Nombre = model.Nombre,
                    Apellidos = model.Apellidos,
                    FechaNacimiento = model.FechaNacimiento,
                    RolId = model.RolId,
                    DepartamentoId = model.DepartamentoId,
                    SalarioBase = model.SalarioBase,
                    ProvinciaId = model.ProvinciaId,
                    CantonId = model.CantonId,
                    DistritoId = model.DistritoId,
                    DireccionExacta = model.DireccionExacta,
                    UsuarioStatus = true,
                    UsuarioCreacion = DateTime.Now
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            await LoadViewBags(); // Si hay errores, cargar las listas nuevamente
            return View(model);
        }

        // Cargar listas para los ViewBags
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