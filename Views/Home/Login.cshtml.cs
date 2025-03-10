using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RRHH.Models;

namespace RRHH.Views.Home
{
    public class LoginModel : PageModel
    {
        private readonly RRHH.Models.DbrrhhContext _context;

        public LoginModel(RRHH.Models.DbrrhhContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CantonId"] = new SelectList(_context.Cantons, "CantonId", "CantonId");
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "DepartamentoId", "DepartamentoId");
            ViewData["DistritoId"] = new SelectList(_context.Distritos, "DistritoId", "DistritoId");
            ViewData["ProvinciaId"] = new SelectList(_context.Provincia, "ProvinciaId", "ProvinciaId");
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId");
                return Page();
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Usuarios.Add(Usuario);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
