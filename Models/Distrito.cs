using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Distrito
{
    public int DistritoId { get; set; }

    public int CantonId { get; set; }

    public string? Codigodistrito { get; set; }

    public string? Distrito1 { get; set; }

    public virtual Canton Canton { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
