using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHH.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    public int Cedula { get; set; }

    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string? Nombre { get; set; }

    [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres.")]
    public string? Apellidos { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public int RolId { get; set; }

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    public int DepartamentoId { get; set; }

    [DataType(DataType.Date, ErrorMessage = "La fecha de nacimiento debe ser una fecha válida.")]
    public DateTime? FechaNacimiento { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El salario base debe ser un número positivo.")]
    public double? SalarioBase { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string NombreUsuario { get; set; } = null!;

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string? Contrasena { get; set; } = null!;

    public string Token { get; set; } = "valor_predeterminado";

    [StringLength(250, ErrorMessage = "La dirección exacta no puede exceder los 250 caracteres.")]
    public string? DireccionExacta { get; set; }

    public int? ProvinciaId { get; set; }

    public int? CantonId { get; set; }

    public int? DistritoId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? UsuarioCreacion { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? UsuarioUpdate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? UsuarioDelete { get; set; }

    public bool UsuarioStatus { get; set; }

    public virtual Canton? Canton { get; set; }

    public virtual Departamento? Departamento { get; set; } = null!;

    public virtual Distrito? Distrito { get; set; }

    public virtual ICollection<HorasExtra> HorasExtras { get; set; } = new List<HorasExtra>();

    public virtual ICollection<Liquidacione> Liquidaciones { get; set; } = new List<Liquidacione>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Marca> Marcas { get; set; } = new List<Marca>();

    public virtual ICollection<Permiso> PermisoUsuarioIdaprobadoPorNavigations { get; set; } = new List<Permiso>();

    public virtual ICollection<Permiso> PermisoUsuarios { get; set; } = new List<Permiso>();

    public virtual ICollection<Planilla> Planillas { get; set; } = new List<Planilla>();

    public virtual Provincium? Provincia { get; set; }

    public virtual Role? Rol { get; set; } = null!;

    public virtual ICollection<Vacacione> Vacaciones { get; set; } = new List<Vacacione>();
}