using System;
using System.Collections.Generic;

namespace RRHH.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public int Cedula { get; set; }

    public string? Nombre { get; set; }

    public string? Apellidos { get; set; }

    public int RolId { get; set; }

    public int DepartamentoId { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public double? SalarioBase { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string? DireccionExacta { get; set; }

    public int? ProvinciaId { get; set; }

    public int? CantonId { get; set; }

    public int? DistritoId { get; set; }

    public DateTime? UsuarioCreacion { get; set; }

    public DateTime? UsuarioUpdate { get; set; }

    public DateTime? UsuarioDelete { get; set; }

    public bool? UsuarioStatus { get; set; }

    public virtual Canton? Canton { get; set; }

    public virtual Departamento Departamento { get; set; } = null!;

    public virtual Distrito? Distrito { get; set; }

    public virtual ICollection<HorasExtra> HorasExtras { get; set; } = new List<HorasExtra>();

    public virtual ICollection<Liquidacione> Liquidaciones { get; set; } = new List<Liquidacione>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Marca> Marcas { get; set; } = new List<Marca>();

    public virtual ICollection<Permiso> PermisoUsuarioIdaprobadoPorNavigations { get; set; } = new List<Permiso>();

    public virtual ICollection<Permiso> PermisoUsuarios { get; set; } = new List<Permiso>();

    public virtual ICollection<Planilla> Planillas { get; set; } = new List<Planilla>();

    public virtual Provincium? Provincia { get; set; }

    public virtual Role Rol { get; set; } = null!;

    public virtual ICollection<Vacacione> Vacaciones { get; set; } = new List<Vacacione>();
}
