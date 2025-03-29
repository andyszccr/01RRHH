using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;
using RRHH.Filters;

namespace RRHH.Controllers
{
    [ServiceFilter(typeof(AutorizacionFilter))]
    public class PuestosController : Controller
    {
        // ... resto del código existente ...
    }
} 