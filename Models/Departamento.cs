using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Departamento
{
    public int DepartamentoId { get; set; }

    public string? Departamento1 { get; set; }

    public DateTime? DepartamentoCreate { get; set; }

    public DateTime? DepartamentoUpdate { get; set; }

    public DateTime? DepartamentoDelete { get; set; }

    public bool DepartamentoStatus { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
