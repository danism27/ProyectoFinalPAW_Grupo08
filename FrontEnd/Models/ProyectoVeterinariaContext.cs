using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    // Renombrar DbSets según las nuevas entidades
    public virtual DbSet<Clinica> Clinicas { get; set; }
    public virtual DbSet<Cita> Citas { get; set; }
    public virtual DbSet<CitaPersonalClinica> CitaPersonalClinicas { get; set; }
    public virtual DbSet<HistorialCita> HistorialCitas { get; set; }
    public virtual DbSet<Notificacion> Notificaciones { get; set; }
    public virtual DbSet<Pago> Pagos { get; set; }
    public virtual DbSet<Personal> Personal { get; set; } // Renombrado de Usuario
    public virtual DbSet<Propietario> Propietarios { get; set; } // Renombrado de Cliente
    public virtual DbSet<Rol> Roles { get; set; } // Renombrado de Role

    // No es necesario reconfigurar la conexión aquí si ya está en Program.cs o appsettings.json
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:CONN");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clinica>(entity =>
        {
            entity.HasKey(e => e.IdClinica).HasName("PK__Sucursal__6CB481891F75FDB9"); // Clave primaria original mantenida

            entity.ToTable("Clinica"); // Renombrar tabla

            entity.Property(e => e.IdClinica).HasColumnName("id_clinica"); // Renombrar columna
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
            entity.HasKey(e => e.IdCita).HasName("PK__Paquete__755269F3A09E1D66"); // Clave primaria original mantenida

            entity.ToTable("Cita"); // Renombrar tabla

            entity.Property(e => e.IdCita).HasColumnName("id_cita"); // Renombrar columna
            entity.Property(e => e.MotivoConsulta) // Renombrado de Descripcion
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("motivo_consulta"); // Renombrar columna
            entity.Property(e => e.DetallesMascota) // Renombrado de Dimensiones
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("detalles_mascota"); // Renombrar columna
            entity.Property(e => e.EstadoCita) // Renombrado de Estado
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_cita"); // Renombrar columna
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaHoraCita) // Renombrado de FechaEntregaEstimada
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora_cita"); // Renombrar columna
            entity.Property(e => e.IdClinicaDestino).HasColumnName("id_clinica_destino"); // Renombrar columna
            entity.Property(e => e.IdClinicaOrigen).HasColumnName("id_clinica_origen"); // Renombrar columna
            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario"); // Renombrar columna y FK
            entity.Property(e => e.IdVeterinarioAsignado).HasColumnName("id_veterinario_asignado"); // Renombrar columna y FK
            entity.Property(e => e.PesoMascota) // Renombrado de Peso
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("peso_mascota"); // Renombrar columna

            // Relaciones (actualizar nombres de propiedades y entidades referenciadas)
            entity.HasOne(d => d.IdClinicaDestinoNavigation).WithMany(p => p.CitaIdClinicaDestinoNavigations)
                .HasForeignKey(d => d.IdClinicaDestino)
                .HasConstraintName("FK__Paquete__id_sucu__48CFD27E"); // Constraint original mantenido

            entity.HasOne(d => d.IdClinicaOrigenNavigation).WithMany(p => p.CitaIdClinicaOrigenNavigations)
                .HasForeignKey(d => d.IdClinicaOrigen)
                .HasConstraintName("FK__Paquete__id_sucu__47DBAE45"); // Constraint original mantenido

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.Citas) // p.Paquetes -> p.Citas
                .HasForeignKey(d => d.IdPropietario) // d.IdCliente -> d.IdPropietario
                .HasConstraintName("FK__Paquete__id_clie__46E78A0C"); // Constraint original mantenido

            entity.HasOne(d => d.IdVeterinarioAsignadoNavigation).WithMany(p => p.CitaIdVeterinarioAsignadoNavigations) // p.PaqueteIdTransportistaAsignadoNavigations -> p.CitaIdVeterinarioAsignadoNavigations
                .HasForeignKey(d => d.IdVeterinarioAsignado) // d.IdTransportistaAsignado -> d.IdVeterinarioAsignado
                .HasConstraintName("FK__Paquete__id_tran__49C3F6B7"); // Constraint original mantenido
        });

        modelBuilder.Entity<CitaPersonalClinica>(entity =>
        {
            entity.HasKey(e => e.IdCitaPersonalClinica).HasName("PK__PaqueteU__A63B5610B93E8DD2"); // Clave primaria original mantenida

            entity.ToTable("CitaPersonalClinica"); // Renombrar tabla

            entity.Property(e => e.IdCitaPersonalClinica).HasColumnName("id_cita_personal_clinica"); // Renombrar columna
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.IdCita).HasColumnName("id_cita"); // Renombrar columna y FK
            entity.Property(e => e.IdClinica).HasColumnName("id_clinica"); // Renombrar columna y FK
            entity.Property(e => e.IdPersonal).HasColumnName("id_personal"); // Renombrar columna y FK

            // Relaciones (actualizar nombres de propiedades y entidades referenciadas)
            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.CitaPersonalClinicas) // p.PaqueteUsuarioSucursals -> p.CitaPersonalClinicas
                .HasForeignKey(d => d.IdCita) // d.IdPaquete -> d.IdCita
                .HasConstraintName("FK__PaqueteUs__id_pa__5441852A"); // Constraint original mantenido

            entity.HasOne(d => d.IdClinicaNavigation).WithMany(p => p.CitaPersonalClinicas) // p.PaqueteUsuarioSucursals -> p.CitaPersonalClinicas
                .HasForeignKey(d => d.IdClinica) // d.IdSucursal -> d.IdClinica
                .HasConstraintName("FK__PaqueteUs__id_su__5629CD9C"); // Constraint original mantenido

            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.CitaPersonalClinicas) // p.PaqueteUsuarioSucursals -> p.CitaPersonalClinicas
                .HasForeignKey(d => d.IdPersonal) // d.IdUsuario -> d.IdPersonal
                .HasConstraintName("FK__PaqueteUs__id_us__5535A963"); // Constraint original mantenido
        });

        modelBuilder.Entity<HistorialCita>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__Historia__D3D8101E774AF533"); // Clave primaria original mantenida

            entity.ToTable("HistorialCita"); // Renombrar tabla

            entity.Property(e => e.IdHistorial).HasColumnName("id_historial"); // Renombrar columna
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
            entity.Property(e => e.IdCita).HasColumnName("id_cita"); // Renombrar columna y FK
            entity.Property(e => e.IdPersonalModificacion).HasColumnName("id_personal_modificacion"); // Renombrar columna y FK
            entity.Property(e => e.UbicacionActual)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ubicacion_actual");

            // Relaciones (actualizar nombres de propiedades y entidades referenciadas)
            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.HistorialCitas) // p.HistorialCambiosPaquetes -> p.HistorialCitas
                .HasForeignKey(d => d.IdCita) // d.IdPaquete -> d.IdCita
                .HasConstraintName("FK__Historial__id_pa__4E88ABD4"); // Constraint original mantenido

            entity.HasOne(d => d.IdPersonalModificacionNavigation).WithMany(p => p.HistorialCitas) // p.HistorialCambiosPaquetes -> p.HistorialCitas
                .HasForeignKey(d => d.IdPersonalModificacion) // d.IdUsuarioModificacion -> d.IdPersonalModificacion
                .HasConstraintName("FK__Historial__id_us__4F7CD00D"); // Constraint original mantenido
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__8E852C9AE53D794E"); // Clave primaria original mantenida

            entity.ToTable("Notificacion"); // Mantener nombre si es genérico

            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_envio");
            entity.Property(e => e.IdPersonal).HasColumnName("id_personal"); // Renombrar columna y FK (asumiendo que va a Personal)
            entity.Property(e => e.Leida)
                .HasDefaultValue(false)
                .HasColumnName("leida");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");

            // Relación (actualizar nombre de propiedad y entidad referenciada)
            // Asumiendo que la notificación es para el personal (antes Usuario)
            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.Notificaciones) // p.Notificacions -> p.Notificaciones
                .HasForeignKey(d => d.IdPersonal) // d.IdUsuario -> d.IdPersonal
                .HasConstraintName("FK__Notificac__id_us__5165187F"); // Constraint original mantenido
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__C5F54C5AE6E038D0"); // Clave primaria original mantenida

            entity.ToTable("Pago"); // Mantener nombre si es genérico

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.IdCita).HasColumnName("id_cita"); // Renombrar columna y FK
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            // Relación (actualizar nombre de propiedad y entidad referenciada)
            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.Pagos) // p.Pagos se mantiene
                .HasForeignKey(d => d.IdCita) // d.IdPaquete -> d.IdCita
                .HasConstraintName("FK__Pago__id_paquete__4CA06362"); // Constraint original mantenido
        });

        modelBuilder.Entity<Personal>(entity => // Renombrar de Usuario
        {
            entity.HasKey(e => e.IdPersonal).HasName("PK__Usuario__63C76BE24D0D9412"); // Clave primaria original mantenida

            entity.ToTable("Personal"); // Renombrar tabla

            entity.Property(e => e.IdPersonal).HasColumnName("id_personal"); // Renombrar columna
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255) // Asumiendo hash
                .IsUnicode(false)
                .HasColumnName("Contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdClinica).HasColumnName("id_clinica"); // Renombrar columna y FK
            entity.Property(e => e.IdRol).HasColumnName("id_rol"); // FK se mantiene
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            // Relaciones (actualizar nombres de propiedades y entidades referenciadas)
            entity.HasOne(d => d.IdClinicaNavigation).WithMany(p => p.PersonalCollection) // p.Usuarios -> p.PersonalCollection
                .HasForeignKey(d => d.IdClinica) // d.IdSucursal -> d.IdClinica
                .HasConstraintName("FK__Usuario__id_sucu__44FF419A"); // Constraint original mantenido

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.PersonalCollection) // p.Usuarios -> p.PersonalCollection
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__id_rol__440B1D61"); // Constraint original mantenido
        });

        modelBuilder.Entity<Propietario>(entity => // Renombrar de Cliente
        {
            entity.HasKey(e => e.IdPropietario).HasName("PK__Cliente__677F38F5F967AE8F"); // Clave primaria original mantenida

            entity.ToTable("Propietario"); // Renombrar tabla

            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario"); // Renombrar columna
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255) // Asumiendo hash
                .IsUnicode(false)
                .HasColumnName("Contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdRol).HasColumnName("id_rol"); // FK se mantiene
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            // Relación (actualizar nombre de propiedad y entidad referenciada)
            entity.HasOne(d => d.Rol).WithMany(p => p.Propietarios) // p.Clientes -> p.Propietarios
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Cliente__id_rol__412EB0B6"); // Constraint original mantenido
        });

        modelBuilder.Entity<Rol>(entity => // Renombrar de Role
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__6AB40045C3E61134"); // Clave primaria original mantenida

            entity.ToTable("Rol"); // Renombrar tabla

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}