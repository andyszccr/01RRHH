using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace RRHH.Controllers
{
    public class LiquidacionesController : Controller
    {
        private readonly DbrrhhContext _context;

        public LiquidacionesController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Liquidaciones
        public async Task<IActionResult> Index()
        {
            var liquidaciones = await _context.Liquidaciones
                .Include(l => l.TipoLiquidacion)
                .Include(l => l.Usuario)
                .ToListAsync();
            return View(liquidaciones);
        }

        // GET: Liquidaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var liquidacione = await _context.Liquidaciones
                .Include(l => l.TipoLiquidacion)
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(m => m.LiquidacionId == id);

            if (liquidacione == null) return NotFound();

            return View(liquidacione);
        }

        // GET: Liquidaciones/Create
        public async Task<IActionResult> Create()
        {
            await LoadViewBags(); // Cargar listas para los ViewBags
            return View();
        }

        // POST: Liquidaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LiquidacionId,UsuarioId,TipoLiquidacionId,FechaLiquidacion,SalarioBase,VacacionesNoDisfrutadas,AguinaldoProporcional,Preaviso,Indemnizacion,TotalLiquidacion,Estado,FechaPago,Banco,NumeroCuenta")] Liquidacione liquidacione)
        {
            // Eliminar errores específicos de ModelState
            ModelState.Remove("Usuario");
            ModelState.Remove("TipoLiquidacion");
            if (ModelState.IsValid)
            {
                _context.Add(liquidacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadViewBags(); // Cargar listas nuevamente si hay errores
            return View(liquidacione);
        }

        // GET: Liquidaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var liquidacione = await _context.Liquidaciones.FindAsync(id);
            if (liquidacione == null) return NotFound();

            await LoadViewBags(); // Cargar listas para los ViewBags
            return View(liquidacione);
        }

        // POST: Liquidaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LiquidacionId,UsuarioId,TipoLiquidacionId,FechaLiquidacion,SalarioBase,VacacionesNoDisfrutadas,AguinaldoProporcional,Preaviso,Indemnizacion,TotalLiquidacion,Estado,FechaPago,Banco,NumeroCuenta")] Liquidacione liquidacione)
        {
            if (id != liquidacione.LiquidacionId) return NotFound();

            // Eliminar errores específicos de ModelState
            ModelState.Remove("Usuario");
            ModelState.Remove("TipoLiquidacion");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(liquidacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LiquidacioneExists(liquidacione.LiquidacionId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadViewBags(); // Cargar listas nuevamente si hay errores
            return View(liquidacione);
        }

        // GET: Liquidaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var liquidacione = await _context.Liquidaciones
                .Include(l => l.TipoLiquidacion)
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(m => m.LiquidacionId == id);

            if (liquidacione == null) return NotFound();

            return View(liquidacione);
        }

        // POST: Liquidaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var liquidacione = await _context.Liquidaciones.FindAsync(id);
            if (liquidacione != null)
            {
                _context.Liquidaciones.Remove(liquidacione);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LiquidacioneExists(int id)
        {
            return _context.Liquidaciones.Any(e => e.LiquidacionId == id);
        }

        // Cargar listas para los ViewBags
        private async Task LoadViewBags()
        {
            ViewBag.UsuarioList = await _context.Usuarios.ToListAsync();
            ViewBag.TipoLiquidacionList = await _context.TipoLiquidaciones.ToListAsync();
        }
    }
}