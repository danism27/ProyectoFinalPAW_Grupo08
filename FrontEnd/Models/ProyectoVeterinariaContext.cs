using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Models;

namespace FrontEnd.Models;

public partial class ProyectoVeterinariaContext : DbContext
{
    public ProyectoVeterinariaContext()
    {
    }

    public ProyectoVeterinariaContext(DbContextOptions<ProyectoVeterinariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clinica> Clinicas { get; set; }
    public virtual DbSet<Cita> Citas { get; set; }
    public virtual DbSet<CitaPersonalClinica> CitaPersonalClinicas { get; set; }
    public virtual DbSet<HistorialCita> HistorialCitas { get; set; }
    public virtual DbSet<Notificacion> Notificaciones { get; set; }
    public virtual DbSet<Pago> Pagos { get; set; }
    public virtual DbSet<Personal> Personal { get; set; }
    public virtual DbSet<Propietario> Propietarios { get; set; }
    public virtual DbSet<Rol> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clinica>(entity =>
        {
            entity.HasKey(e => e.IdClinica).HasName("PK__Sucursal__6CB481891F75FDB9");
            entity.ToTable("Clinica");
            entity.Property(e => e.IdClinica).HasColumnName("id_clinica");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.IdCita).HasName("PK__Paquete__755269F3A09E1D66");
            entity.ToTable("Cita");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.MotivoConsulta)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("motivo_consulta");
            entity.Property(e => e.DetallesMascota)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("detalles_mascota");
            entity.Property(e => e.EstadoCita)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_cita");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaHoraCita)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora_cita");
            entity.Property(e => e.IdClinicaDestino).HasColumnName("id_clinica_destino");
            entity.Property(e => e.IdClinicaOrigen).HasColumnName("id_clinica_origen");
            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario");
            entity.Property(e => e.IdVeterinarioAsignado).HasColumnName("id_veterinario_asignado");
            entity.Property(e => e.PesoMascota)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("peso_mascota");

            entity.HasOne(d => d.IdClinicaDestinoNavigation).WithMany(p => p.CitaIdClinicaDestinoNavigations)
                .HasForeignKey(d => d.IdClinicaDestino)
                .HasConstraintName("FK__Paquete__id_sucu__48CFD27E");

            entity.HasOne(d => d.IdClinicaOrigenNavigation).WithMany(p => p.CitaIdClinicaOrigenNavigations)
                .HasForeignKey(d => d.IdClinicaOrigen)
                .HasConstraintName("FK__Paquete__id_sucu__47DBAE45");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.Citas)
                .HasForeignKey(d => d.IdPropietario)
                .HasConstraintName("FK__Paquete__id_clie__46E78A0C");

            entity.HasOne(d => d.IdVeterinarioAsignadoNavigation).WithMany(p => p.CitaIdVeterinarioAsignadoNavigations)
                .HasForeignKey(d => d.IdVeterinarioAsignado)
                .HasConstraintName("FK__Paquete__id_tran__49C3F6B7");
        });

        modelBuilder.Entity<CitaPersonalClinica>(entity =>
        {
            entity.HasKey(e => e.IdCitaPersonalClinica).HasName("PK__PaqueteU__A63B5610B93E8DD2");
            entity.ToTable("CitaPersonalClinica");
            entity.Property(e => e.IdCitaPersonalClinica).HasColumnName("id_cita_personal_clinica");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.IdClinica).HasColumnName("id_clinica");
            entity.Property(e => e.IdPersonal).HasColumnName("id_personal");

            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.CitaPersonalClinicas)
                .HasForeignKey(d => d.IdCita)
                .HasConstraintName("FK__PaqueteUs__id_pa__5441852A");

            entity.HasOne(d => d.IdClinicaNavigation).WithMany(p => p.CitaPersonalClinicas)
                .HasForeignKey(d => d.IdClinica)
                .HasConstraintName("FK__PaqueteUs__id_su__5629CD9C");

            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.CitaPersonalClinicas)
                .HasForeignKey(d => d.IdPersonal)
                .HasConstraintName("FK__PaqueteUs__id_us__5535A963");
        });

        modelBuilder.Entity<HistorialCita>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__Historia__D3D8101E774AF533");
            entity.ToTable("HistorialCita");
            entity.Property(e => e.IdHistorial).HasColumnName("id_historial");
            entity.Property(e => e.EstadoAnterior)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_anterior");
            entity.Property(e => e.EstadoNuevo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_nuevo");
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_cambio");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.IdPersonalModificacion).HasColumnName("id_personal_modificacion");
            entity.Property(e => e.UbicacionActual)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ubicacion_actual");

            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.HistorialCitas)
                .HasForeignKey(d => d.IdCita)
                .HasConstraintName("FK__Historial__id_pa__4E88ABD4");

            entity.HasOne(d => d.IdPersonalModificacionNavigation).WithMany(p => p.HistorialCitas)
                .HasForeignKey(d => d.IdPersonalModificacion)
                .HasConstraintName("FK__Historial__id_us__4F7CD00D");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__8E852C9AE53D794E");
            entity.ToTable("Notificacion");
            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_envio");
            entity.Property(e => e.IdPersonal).HasColumnName("id_personal");
            entity.Property(e => e.Leida)
                .HasDefaultValue(false)
                .HasColumnName("leida");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");

            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdPersonal)
                .HasConstraintName("FK__Notificac__id_us__5165187F");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__C5F54C5AE6E038D0");
            entity.ToTable("Pago");
            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdCita)
                .HasConstraintName("FK__Pago__id_paquete__4CA06362");
        });

        modelBuilder.Entity<Personal>(entity =>
        {
            entity.HasKey(e => e.IdPersonal).HasName("PK__Usuario__63C76BE24D0D9412");
            entity.ToTable("Personal");
            entity.Property(e => e.IdPersonal).HasColumnName("id_personal");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdClinica).HasColumnName("id_clinica");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdClinicaNavigation).WithMany(p => p.PersonalCollection)
                .HasForeignKey(d => d.IdClinica)
                .HasConstraintName("FK__Usuario__id_sucu__44FF419A");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.PersonalCollection)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__id_rol__440B1D61");
        });

        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.HasKey(e => e.IdPropietario).HasName("PK__Cliente__677F38F5F967AE8F");
            entity.ToTable("Propietario");
            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.Rol).WithMany(p => p.Propietarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Cliente__id_rol__412EB0B6");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__6AB40045C3E61134");
            entity.ToTable("Rol");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_rol");

            // --- SIEMBRA DE DATOS PARA LA ENTIDAD ROL ---
            entity.HasData(
                new Rol { IdRol = 1, NombreRol = "Cliente" },
                new Rol { IdRol = 2, NombreRol = "Asistente" },
                new Rol { IdRol = 3, NombreRol = "Administrador" },
                new Rol { IdRol = 4, NombreRol = "Veterinario" }
            );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}