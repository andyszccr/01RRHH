using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using RRHH.Models;
using RRHH.Views.Home;
using System.Diagnostics;

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
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Usuario model)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = model.NombreUsuario,
                    Contrasena = model.Contrasena,
                    // Asigna otros campos necesarios
                };

                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }
    }
}
