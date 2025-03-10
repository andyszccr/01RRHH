using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Log
{
    public int LogsId { get; set; }

    public DateTime Fecha { get; set; }

    public int? UsuarioId { get; set; }

    public string? Modulo { get; set; }

    public string? DetallesAdicionales { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
