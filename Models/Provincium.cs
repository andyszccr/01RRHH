using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Provincium
{
    public int ProvinciaId { get; set; }

    public string? Provincia { get; set; }

    public virtual ICollection<Canton> Cantons { get; set; } = new List<Canton>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
