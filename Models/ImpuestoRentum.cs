using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class ImpuestoRentum
{
    public int ImpuestoRentaId { get; set; }

    public string RangoIngreso { get; set; } = null!;

    public double? Porcentaje { get; set; }

    public virtual ICollection<Planilla> Planillas { get; set; } = new List<Planilla>();
}
