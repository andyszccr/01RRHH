using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class TipoDeduccione
{
    public int TipoDeduccionId { get; set; }

    public string? DeduccionNombre { get; set; }

    public double? DeduccionPorcentaje { get; set; }

    public virtual ICollection<Planilla> Planillas { get; set; } = new List<Planilla>();
}
