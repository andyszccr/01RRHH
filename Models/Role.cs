using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Role
{
    public int RolId { get; set; }

    public string? NombreRol { get; set; }

    public DateTime? RolCreate { get; set; }

    public DateTime? RolUpdate { get; set; }

    public DateTime? RolDelete { get; set; }

    public bool? RolStatus { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
