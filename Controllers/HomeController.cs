using Microsoft.AspNetCore.Mvc;
using RRHH.Models;
using RRHH.Views.Home;
using System.Diagnostics;

namespace RRHH.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        Usuario usuario =new Usuario();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //**************************************************************************************************
        // Acci�n GET para mostrar el formulario de login
        public IActionResult Login()
        {
            // Si quieres pasar un modelo vac�o a la vista, aseg�rate de inicializarlo
            return View(usuario);  // O puede ser null si necesitas un modelo vac�o
        }

        // Acci�n POST para manejar el login cuando se env�e el formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // L�gica de autenticaci�n aqu�
                return RedirectToAction("Index", "Home");
            }
            return View(model);  // Si hay errores, devolver el formulario con mensajes de error
        }
        //**************************************************************************************************
    }
}
