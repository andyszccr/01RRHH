using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RRHH.Models;

namespace RRHH.Controllers
{
    public class DepartamentoesController : Controller
    {
        private readonly DbrrhhContext _context;

        public DepartamentoesController(DbrrhhContext context)
        {
            _context = context;
        }

        // GET: Departamentoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departamentos.ToListAsync());
        }

        // GET: Departamentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(m => m.DepartamentoId == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // GET: Departamentoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departamentoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartamentoId,Departamento1,DepartamentoStatus")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                departamento.DepartamentoCreate = DateTime.Now; // Asignar fecha de creación
                //departamento.DepartamentoUpdate = DateTime.Now; // Asignar fecha de actualización
                departamento.DepartamentoDelete = null; // No hay fecha de eliminación al crear
                departamento.DepartamentoStatus = true; // Activo
                _context.Add(departamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // GET: Departamentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        // POST: Departamentoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartamentoId,Departamento1,DepartamentoStatus")] Departamento departamento)
        {
            if (id != departamento.DepartamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDepartamento = await _context.Departamentos.FindAsync(id);
                    if (existingDepartamento == null)
                    {
                        return NotFound();
                    }

                    // No modificar la fecha de creación
                    departamento.DepartamentoCreate = existingDepartamento.DepartamentoCreate;

                    // Asignar la fecha actual al actualizar
                    departamento.DepartamentoUpdate = DateTime.Now;

                    // Actualizar el estado
                    departamento.DepartamentoStatus = departamento.DepartamentoStatus;

                    _context.Entry(existingDepartamento).CurrentValues.SetValues(departamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(departamento.DepartamentoId))
                    {
                        return NotFound();
                    }
                    throw; // Re-lanzar la excepción si ocurre un error
                }
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // GET: Departamentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(m => m.DepartamentoId == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // POST: Departamentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento != null)
            {
                departamento.DepartamentoDelete = DateTime.Now; // Asignar fecha de eliminación
                departamento.DepartamentoStatus = false; // Cambiar estado a inactivo
                _context.Departamentos.Update(departamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartamentoExists(int id) => _context.Departamentos.Any(e => e.DepartamentoId == id);
    }
}