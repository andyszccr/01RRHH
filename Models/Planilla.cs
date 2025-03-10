using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Planilla
{
    public int PlanillaId { get; set; }

    public DateTime FechaPlanilla { get; set; }

    public int UsuarioId { get; set; }

    public int? HorasTrabajadas { get; set; }

    public double? HorasExtras { get; set; }

    public int TipoDeduccionId { get; set; }

    public double? Deducciones { get; set; }

    public int? ImpuestoRentaId { get; set; }

    public double? ImpuestoRenta { get; set; }

    public double? SalarioNeto { get; set; }

    public DateTime? PlanillaFechaPago { get; set; }

    public int? PlanillaStatus { get; set; }

    public string? Banco { get; set; }

    public string? NumeroCuenta { get; set; }

    public virtual ImpuestoRentum? ImpuestoRentaNavigation { get; set; }

    public virtual TipoDeduccione TipoDeduccion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
