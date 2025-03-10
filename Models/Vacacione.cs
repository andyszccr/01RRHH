using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Vacacione
{
    public int VacacionId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime? FechaSolicitud { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int DiasSolicitados { get; set; }

    public int? DiasAprobados { get; set; }

    public string Estado { get; set; } = null!;

    public string? Comentarios { get; set; }

    public DateTime? FechaAprobacion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public decimal? SalarioVacaciones { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
