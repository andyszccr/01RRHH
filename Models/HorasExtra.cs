using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class HorasExtra
{
    public int HorasExtraId { get; set; }

    public int UsuarioId { get; set; }

    public int TipoJornadaId { get; set; }

    public DateTime Fecha { get; set; }

    public string TipoPago { get; set; } = null!;

    public int HorasExtra1 { get; set; }

    public double? MontoPagoSalario { get; set; }

    public string Motivo { get; set; } = null!;

    public virtual TipoJornadum TipoJornada { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
