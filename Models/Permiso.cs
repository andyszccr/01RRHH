using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHH.Models;

public partial class Permiso
{
    public int PermisoId { get; set; }

    [Required(ErrorMessage = "El solicitante es requerido")]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El tipo de permiso es requerido")]
    public int TipoPermisoId { get; set; }

    public int? PermisoStatus { get; set; }

    [Required(ErrorMessage = "Las horas solicitadas son requeridas")]
    [Range(0.5, 24, ErrorMessage = "Las horas solicitadas deben estar entre 0.5 y 24")]
    public int HorasSolicitadas { get; set; }

    public DateTime? PermisoCreacion { get; set; }

    public DateTime? PermisoUpdate { get; set; }

    public DateTime? PermisoDelete { get; set; }

    public int? UsuarioIdaprobadoPor { get; set; }

    [Required(ErrorMessage = "El motivo es requerido")]
    [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres")]
    public string Motivo { get; set; } = null!;

    public virtual TipoPermiso TipoPermiso { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual Usuario? UsuarioIdaprobadoPorNavigation { get; set; }
}
