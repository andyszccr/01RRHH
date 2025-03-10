using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Permiso
{
    public int PermisoId { get; set; }

    public int UsuarioId { get; set; }

    public int TipoPermisoId { get; set; }

    public int? PermisoStatus { get; set; }

    public int HorasSolicitadas { get; set; }

    public DateTime? PermisoCreacion { get; set; }

    public DateTime? PermisoUpdate { get; set; }

    public DateTime? PermisoDelete { get; set; }

    public int? UsuarioIdaprobadoPor { get; set; }

    public string Motivo { get; set; } = null!;

    public virtual TipoPermiso TipoPermiso { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual Usuario? UsuarioIdaprobadoPorNavigation { get; set; }
}
