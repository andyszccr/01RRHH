using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Marca
{
    public int MarcaId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime Fecha { get; set; }

    public DateTime HoraEntrada { get; set; }

    public DateTime? HoraSalida { get; set; }

    public DateTime? MarcaCreacion { get; set; }

    public DateTime? MarcaUpdate { get; set; }

    public DateTime? MarcaDelete { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
