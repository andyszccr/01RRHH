using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Liquidacione
{
    public int LiquidacionId { get; set; }

    public int UsuarioId { get; set; }

    public int TipoLiquidacionId { get; set; }

    public DateOnly FechaLiquidacion { get; set; }

    public double SalarioBase { get; set; }

    public decimal? VacacionesNoDisfrutadas { get; set; }

    public decimal? AguinaldoProporcional { get; set; }

    public decimal? Preaviso { get; set; }

    public decimal? Indemnizacion { get; set; }

    public decimal TotalLiquidacion { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaPago { get; set; }

    public string? Banco { get; set; }

    public string? NumeroCuenta { get; set; }

    public virtual TipoLiquidacione TipoLiquidacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
