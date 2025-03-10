using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class TipoJornadum
{
    public int TipoJornadaId { get; set; }

    public string TipoJornada { get; set; } = null!;

    public virtual ICollection<HorasExtra> HorasExtras { get; set; } = new List<HorasExtra>();
}
