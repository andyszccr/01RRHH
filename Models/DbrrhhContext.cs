using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RRHH.Models;

public partial class DbrrhhContext : DbContext
{
    public DbrrhhContext()
    {
    }

    public DbrrhhContext(DbContextOptions<DbrrhhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Canton> Cantons { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Distrito> Distritos { get; set; }

    public virtual DbSet<HorasExtra> HorasExtras { get; set; }

    public virtual DbSet<ImpuestoRentum> ImpuestoRenta { get; set; }

    public virtual DbSet<Liquidacione> Liquidaciones { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Planilla> Planillas { get; set; }

    public virtual DbSet<Provincium> Provincia { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TipoDeduccione> TipoDeducciones { get; set; }

    public virtual DbSet<TipoJornadum> TipoJornada { get; set; }

    public virtual DbSet<TipoLiquidacione> TipoLiquidaciones { get; set; }

    public virtual DbSet<TipoPermiso> TipoPermisos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vacacione> Vacaciones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=NULL\\SQLEXPRESS; database=DBRRHH; integrated security=true; TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Canton>(entity =>
        {
            entity.HasKey(e => e.CantonId).HasName("PK__Canton__5536C9D88085FED0");

            entity.ToTable("Canton");

            entity.Property(e => e.CantonId).HasColumnName("CantonID");
            entity.Property(e => e.Canton1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("canton");
            entity.Property(e => e.CantonCodigo)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ProvinciaId).HasColumnName("provinciaID");

            entity.HasOne(d => d.Provincia).WithMany(p => p.Cantons)
                .HasForeignKey(d => d.ProvinciaId)
                .HasConstraintName("FK_Canton_provincia");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.Property(e => e.DepartamentoId).HasColumnName("DepartamentoID");
            entity.Property(e => e.Departamento1).HasColumnName("Departamento");
            entity.Property(e => e.DepartamentoCreate).HasColumnType("datetime");
            entity.Property(e => e.DepartamentoDelete).HasColumnType("datetime");
            entity.Property(e => e.DepartamentoStatus).HasDefaultValue(true);
            entity.Property(e => e.DepartamentoUpdate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Distrito>(entity =>
        {
            entity.HasKey(e => e.DistritoId).HasName("PK__Distrito__BE6ADABD4A560968");

            entity.ToTable("Distrito");

            entity.Property(e => e.DistritoId).HasColumnName("DistritoID");
            entity.Property(e => e.CantonId).HasColumnName("CantonID");
            entity.Property(e => e.Codigodistrito)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Distrito1)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("distrito");

            entity.HasOne(d => d.Canton).WithMany(p => p.Distritos)
                .HasForeignKey(d => d.CantonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Distrito_Canton");
        });

        modelBuilder.Entity<HorasExtra>(entity =>
        {
            entity.HasKey(e => e.HorasExtraId).HasName("PK__HorasExt__4C17000A597E7B5C");

            entity.ToTable("HorasExtra");

            entity.Property(e => e.HorasExtraId).HasColumnName("HorasExtraID");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.HorasExtra1).HasColumnName("HorasExtra");
            entity.Property(e => e.Motivo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoJornadaId).HasColumnName("TipoJornadaID");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.TipoJornada).WithMany(p => p.HorasExtras)
                .HasForeignKey(d => d.TipoJornadaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HorasExtra_TipoJornada");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HorasExtras)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HorasExtra_Usuarios");
        });

        modelBuilder.Entity<ImpuestoRentum>(entity =>
        {
            entity.HasKey(e => e.ImpuestoRentaId).HasName("PK__Impuesto__DFEDD7C2960DD070");

            entity.Property(e => e.ImpuestoRentaId).HasColumnName("ImpuestoRentaID");
            entity.Property(e => e.RangoIngreso)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Liquidacione>(entity =>
        {
            entity.HasKey(e => e.LiquidacionId).HasName("PK__Liquidac__94C125AC8F981D40");

            entity.Property(e => e.LiquidacionId).HasColumnName("LiquidacionID");
            entity.Property(e => e.AguinaldoProporcional).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Banco)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.Indemnizacion).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Preaviso).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalLiquidacion).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.VacacionesNoDisfrutadas).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.TipoLiquidacion).WithMany(p => p.Liquidaciones)
                .HasForeignKey(d => d.TipoLiquidacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Liquidaciones_TipoLiquidaciones");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Liquidaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Liquidaciones_Usuarios");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogsId).HasName("PK__logs__C92054AE17432D4D");

            entity.ToTable("logs");

            entity.Property(e => e.LogsId).HasColumnName("LogsID");
            entity.Property(e => e.DetallesAdicionales)
                .IsUnicode(false)
                .HasColumnName("detalles_adicionales");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Modulo)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_logs_Usuarios");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.HoraEntrada).HasColumnType("datetime");
            entity.Property(e => e.HoraSalida).HasColumnType("datetime");
            entity.Property(e => e.MarcaCreacion).HasColumnType("datetime");
            entity.Property(e => e.MarcaDelete).HasColumnType("datetime");
            entity.Property(e => e.MarcaUpdate).HasColumnType("datetime");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Marcas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Marcas_Usuarios");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.PermisoId).HasName("PK__Permisos__96E0C7233E6CD94C");

            entity.Property(e => e.Motivo).IsUnicode(false);
            entity.Property(e => e.PermisoCreacion).HasColumnType("datetime");
            entity.Property(e => e.PermisoDelete).HasColumnType("datetime");
            entity.Property(e => e.PermisoUpdate).HasColumnType("datetime");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.UsuarioIdaprobadoPor).HasColumnName("UsuarioIDAprobadoPor");

            entity.HasOne(d => d.TipoPermiso).WithMany(p => p.Permisos)
                .HasForeignKey(d => d.TipoPermisoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permisos_TipoPermisos");

            entity.HasOne(d => d.Usuario).WithMany(p => p.PermisoUsuarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permisos_Usuarios");

            entity.HasOne(d => d.UsuarioIdaprobadoPorNavigation).WithMany(p => p.PermisoUsuarioIdaprobadoPorNavigations)
                .HasForeignKey(d => d.UsuarioIdaprobadoPor)
                .HasConstraintName("FK_Permisos_Usuarios1");
        });

        modelBuilder.Entity<Planilla>(entity =>
        {
            entity.HasKey(e => e.PlanillaId).HasName("PK__Planilla__D6603A4A23F2DD0C");

            entity.ToTable("Planilla");

            entity.Property(e => e.PlanillaId).HasColumnName("PlanillaID");
            entity.Property(e => e.Banco)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaPlanilla).HasColumnType("datetime");
            entity.Property(e => e.ImpuestoRentaId).HasColumnName("ImpuestoRentaID");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlanillaFechaPago).HasColumnType("datetime");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.ImpuestoRentaNavigation).WithMany(p => p.Planillas)
                .HasForeignKey(d => d.ImpuestoRentaId)
                .HasConstraintName("FK_Planilla_ImpuestoRenta");

            entity.HasOne(d => d.TipoDeduccion).WithMany(p => p.Planillas)
                .HasForeignKey(d => d.TipoDeduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Planilla_TipoDeducciones");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Planillas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Planilla_Usuarios");
        });

        modelBuilder.Entity<Provincium>(entity =>
        {
            entity.HasKey(e => e.ProvinciaId).HasName("PK__provinci__671F1345E72FD02C");

            entity.ToTable("provincia");

            entity.Property(e => e.ProvinciaId).HasColumnName("provinciaID");
            entity.Property(e => e.Provincia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("provincia");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId);

            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.RolCreate).HasColumnType("datetime");
            entity.Property(e => e.RolDelete).HasColumnType("datetime");
            entity.Property(e => e.RolStatus).HasDefaultValue(true);
            entity.Property(e => e.RolUpdate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TipoDeduccione>(entity =>
        {
            entity.HasKey(e => e.TipoDeduccionId).HasName("PK__TipoDedu__B0C52F3F92EC01E7");

            entity.Property(e => e.DeduccionNombre)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoJornadum>(entity =>
        {
            entity.HasKey(e => e.TipoJornadaId).HasName("PK__TipoJorn__D5BB58BCCF11B035");

            entity.Property(e => e.TipoJornadaId).HasColumnName("TipoJornadaID");
            entity.Property(e => e.TipoJornada)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoLiquidacione>(entity =>
        {
            entity.HasKey(e => e.TipoLiquidacionId).HasName("PK__TipoLiqu__9F5B24B95F39A53B");

            entity.Property(e => e.NombreTipo).IsUnicode(false);
        });

        modelBuilder.Entity<TipoPermiso>(entity =>
        {
            entity.HasKey(e => e.TipoPermisoId).HasName("PK__TipoPerm__6A7B92F2199348D8");

            entity.Property(e => e.TipoPermisoId).HasColumnName("TipoPermisoID");
            entity.Property(e => e.TipoPermiso1)
                .IsUnicode(false)
                .HasColumnName("TipoPermiso");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798F4E3D464");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CantonId).HasColumnName("CantonID");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.DepartamentoId).HasColumnName("DepartamentoID");
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DistritoId).HasColumnName("DistritoID");
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ProvinciaId).HasColumnName("provinciaID");
            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioCreacion).HasColumnType("datetime");
            entity.Property(e => e.UsuarioDelete).HasColumnType("datetime");
            entity.Property(e => e.UsuarioUpdate).HasColumnType("datetime");

            entity.HasOne(d => d.Canton).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.CantonId)
                .HasConstraintName("FK_Usuarios_Canton");

            entity.HasOne(d => d.Departamento).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.DepartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Departamentos");

            entity.HasOne(d => d.Distrito).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.DistritoId)
                .HasConstraintName("FK_Usuarios_Distrito");

            entity.HasOne(d => d.Provincia).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.ProvinciaId)
                .HasConstraintName("FK_Usuarios_provincia");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<Vacacione>(entity =>
        {
            entity.HasKey(e => e.VacacionId).HasName("PK__Vacacion__CEAEE9FA069AB07F");

            entity.Property(e => e.VacacionId).HasColumnName("VacacionID");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaAprobacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitud).HasColumnType("datetime");
            entity.Property(e => e.SalarioVacaciones).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Vacaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacaciones_Usuarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
