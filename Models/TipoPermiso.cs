using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class TipoPermiso
{
    public int TipoPermisoId { get; set; }

    public string? TipoPermiso1 { get; set; }

    public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
}
