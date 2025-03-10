using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Canton
{
    public int CantonId { get; set; }

    public int? ProvinciaId { get; set; }

    public string? CantonCodigo { get; set; }

    public string? Canton1 { get; set; }

    public virtual ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();

    public virtual Provincium? Provincia { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
