using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class TipoLiquidacione
{
    public int TipoLiquidacionId { get; set; }

    public string? NombreTipo { get; set; }

    public virtual ICollection<Liquidacione> Liquidaciones { get; set; } = new List<Liquidacione>();
}
