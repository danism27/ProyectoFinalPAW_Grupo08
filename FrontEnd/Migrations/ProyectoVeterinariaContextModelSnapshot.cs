﻿// <auto-generated />
using System;
using FrontEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FrontEnd.Migrations
{
    [DbContext(typeof(ProyectoVeterinariaContext))]
    partial class ProyectoVeterinariaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FrontEnd.Models.Cita", b =>
                {
                    b.Property<int>("IdCita")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_cita");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCita"));

                    b.Property<string>("DetallesMascota")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("detalles_mascota");

                    b.Property<string>("EstadoCita")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("estado_cita");

                    b.Property<DateTime?>("FechaCreacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("fecha_creacion")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("FechaHoraCita")
                        .HasColumnType("datetime")
                        .HasColumnName("fecha_hora_cita");

                    b.Property<int?>("IdClinicaDestino")
                        .HasColumnType("int")
                        .HasColumnName("id_clinica_destino");

                    b.Property<int?>("IdClinicaOrigen")
                        .HasColumnType("int")
                        .HasColumnName("id_clinica_origen");

                    b.Property<int?>("IdPropietario")
                        .HasColumnType("int")
                        .HasColumnName("id_propietario");

                    b.Property<int?>("IdVeterinarioAsignado")
                        .HasColumnType("int")
                        .HasColumnName("id_veterinario_asignado");

                    b.Property<string>("MotivoConsulta")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("motivo_consulta");

                    b.Property<decimal?>("PesoMascota")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("peso_mascota");

                    b.HasKey("IdCita")
                        .HasName("PK__Paquete__755269F3A09E1D66");

                    b.HasIndex("IdClinicaDestino");

                    b.HasIndex("IdClinicaOrigen");

                    b.HasIndex("IdPropietario");

                    b.HasIndex("IdVeterinarioAsignado");

                    b.ToTable("Cita", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.CitaPersonalClinica", b =>
                {
                    b.Property<int>("IdCitaPersonalClinica")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_cita_personal_clinica");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCitaPersonalClinica"));

                    b.Property<DateTime?>("FechaAsignacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("fecha_asignacion")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("IdCita")
                        .HasColumnType("int")
                        .HasColumnName("id_cita");

                    b.Property<int?>("IdClinica")
                        .HasColumnType("int")
                        .HasColumnName("id_clinica");

                    b.Property<int?>("IdPersonal")
                        .HasColumnType("int")
                        .HasColumnName("id_personal");

                    b.HasKey("IdCitaPersonalClinica")
                        .HasName("PK__PaqueteU__A63B5610B93E8DD2");

                    b.HasIndex("IdCita");

                    b.HasIndex("IdClinica");

                    b.HasIndex("IdPersonal");

                    b.ToTable("CitaPersonalClinica", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Clinica", b =>
                {
                    b.Property<int>("IdClinica")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_clinica");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdClinica"));

                    b.Property<string>("Direccion")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("direccion");

                    b.Property<string>("Nombre")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("nombre");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("telefono");

                    b.HasKey("IdClinica")
                        .HasName("PK__Sucursal__6CB481891F75FDB9");

                    b.ToTable("Clinica", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.HistorialCita", b =>
                {
                    b.Property<int>("IdHistorial")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_historial");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdHistorial"));

                    b.Property<string>("EstadoAnterior")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("estado_anterior");

                    b.Property<string>("EstadoNuevo")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("estado_nuevo");

                    b.Property<DateTime?>("FechaCambio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("fecha_cambio")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("IdCita")
                        .HasColumnType("int")
                        .HasColumnName("id_cita");

                    b.Property<int?>("IdPersonalModificacion")
                        .HasColumnType("int")
                        .HasColumnName("id_personal_modificacion");

                    b.Property<string>("UbicacionActual")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("ubicacion_actual");

                    b.HasKey("IdHistorial")
                        .HasName("PK__Historia__D3D8101E774AF533");

                    b.HasIndex("IdCita");

                    b.HasIndex("IdPersonalModificacion");

                    b.ToTable("HistorialCita", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Notificacion", b =>
                {
                    b.Property<int>("IdNotificacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_notificacion");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdNotificacion"));

                    b.Property<DateTime?>("FechaEnvio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("fecha_envio")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("IdPersonal")
                        .HasColumnType("int")
                        .HasColumnName("id_personal");

                    b.Property<bool?>("Leida")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("leida");

                    b.Property<string>("Mensaje")
                        .HasColumnType("text")
                        .HasColumnName("mensaje");

                    b.HasKey("IdNotificacion")
                        .HasName("PK__Notifica__8E852C9AE53D794E");

                    b.HasIndex("IdPersonal");

                    b.ToTable("Notificacion", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Pago", b =>
                {
                    b.Property<int>("IdPago")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_pago");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPago"));

                    b.Property<DateTime?>("FechaPago")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("fecha_pago")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("IdCita")
                        .HasColumnType("int")
                        .HasColumnName("id_cita");

                    b.Property<string>("MetodoPago")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("metodo_pago");

                    b.Property<decimal?>("Monto")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("monto");

                    b.HasKey("IdPago")
                        .HasName("PK__Pago__C5F54C5AE6E038D0");

                    b.HasIndex("IdCita");

                    b.ToTable("Pago", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Personal", b =>
                {
                    b.Property<int>("IdPersonal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_personal");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPersonal"));

                    b.Property<string>("Apellido")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("apellido");

                    b.Property<string>("Contrasenna")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("contrasenna");

                    b.Property<string>("Correo")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("correo");

                    b.Property<string>("FotoPerfil")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("foto_perfil");

                    b.Property<int?>("IdClinica")
                        .HasColumnType("int")
                        .HasColumnName("id_clinica");

                    b.Property<int?>("IdRol")
                        .HasColumnType("int")
                        .HasColumnName("id_rol");

                    b.Property<string>("Nombre")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("nombre");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("telefono");

                    b.HasKey("IdPersonal")
                        .HasName("PK__Usuario__63C76BE24D0D9412");

                    b.HasIndex("IdClinica");

                    b.HasIndex("IdRol");

                    b.ToTable("Personal", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Propietario", b =>
                {
                    b.Property<int>("IdPropietario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_propietario");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPropietario"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("apellido");

                    b.Property<string>("Contrasenna")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("contrasenna");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("correo");

                    b.Property<int?>("IdRol")
                        .HasColumnType("int")
                        .HasColumnName("id_rol");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("nombre");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("telefono");

                    b.HasKey("IdPropietario")
                        .HasName("PK__Cliente__677F38F5F967AE8F");

                    b.HasIndex("IdRol");

                    b.ToTable("Propietario", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Rol", b =>
                {
                    b.Property<int>("IdRol")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_rol");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRol"));

                    b.Property<string>("NombreRol")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("nombre_rol");

                    b.HasKey("IdRol")
                        .HasName("PK__Roles__6AB40045C3E61134");

                    b.ToTable("Rol", (string)null);
                });

            modelBuilder.Entity("FrontEnd.Models.Cita", b =>
                {
                    b.HasOne("FrontEnd.Models.Clinica", "IdClinicaDestinoNavigation")
                        .WithMany("CitaIdClinicaDestinoNavigations")
                        .HasForeignKey("IdClinicaDestino")
                        .HasConstraintName("FK__Paquete__id_sucu__48CFD27E");

                    b.HasOne("FrontEnd.Models.Clinica", "IdClinicaOrigenNavigation")
                        .WithMany("CitaIdClinicaOrigenNavigations")
                        .HasForeignKey("IdClinicaOrigen")
                        .HasConstraintName("FK__Paquete__id_sucu__47DBAE45");

                    b.HasOne("FrontEnd.Models.Propietario", "IdPropietarioNavigation")
                        .WithMany("Citas")
                        .HasForeignKey("IdPropietario")
                        .HasConstraintName("FK__Paquete__id_clie__46E78A0C");

                    b.HasOne("FrontEnd.Models.Personal", "IdVeterinarioAsignadoNavigation")
                        .WithMany("CitaIdVeterinarioAsignadoNavigations")
                        .HasForeignKey("IdVeterinarioAsignado")
                        .HasConstraintName("FK__Paquete__id_tran__49C3F6B7");

                    b.Navigation("IdClinicaDestinoNavigation");

                    b.Navigation("IdClinicaOrigenNavigation");

                    b.Navigation("IdPropietarioNavigation");

                    b.Navigation("IdVeterinarioAsignadoNavigation");
                });

            modelBuilder.Entity("FrontEnd.Models.CitaPersonalClinica", b =>
                {
                    b.HasOne("FrontEnd.Models.Cita", "IdCitaNavigation")
                        .WithMany("CitaPersonalClinicas")
                        .HasForeignKey("IdCita")
                        .HasConstraintName("FK__PaqueteUs__id_pa__5441852A");

                    b.HasOne("FrontEnd.Models.Clinica", "IdClinicaNavigation")
                        .WithMany("CitaPersonalClinicas")
                        .HasForeignKey("IdClinica")
                        .HasConstraintName("FK__PaqueteUs__id_su__5629CD9C");

                    b.HasOne("FrontEnd.Models.Personal", "IdPersonalNavigation")
                        .WithMany("CitaPersonalClinicas")
                        .HasForeignKey("IdPersonal")
                        .HasConstraintName("FK__PaqueteUs__id_us__5535A963");

                    b.Navigation("IdCitaNavigation");

                    b.Navigation("IdClinicaNavigation");

                    b.Navigation("IdPersonalNavigation");
                });

            modelBuilder.Entity("FrontEnd.Models.HistorialCita", b =>
                {
                    b.HasOne("FrontEnd.Models.Cita", "IdCitaNavigation")
                        .WithMany("HistorialCitas")
                        .HasForeignKey("IdCita")
                        .HasConstraintName("FK__Historial__id_pa__4E88ABD4");

                    b.HasOne("FrontEnd.Models.Personal", "IdPersonalModificacionNavigation")
                        .WithMany("HistorialCitas")
                        .HasForeignKey("IdPersonalModificacion")
                        .HasConstraintName("FK__Historial__id_us__4F7CD00D");

                    b.Navigation("IdCitaNavigation");

                    b.Navigation("IdPersonalModificacionNavigation");
                });

            modelBuilder.Entity("FrontEnd.Models.Notificacion", b =>
                {
                    b.HasOne("FrontEnd.Models.Personal", "IdPersonalNavigation")
                        .WithMany("Notificaciones")
                        .HasForeignKey("IdPersonal")
                        .HasConstraintName("FK__Notificac__id_us__5165187F");

                    b.Navigation("IdPersonalNavigation");
                });

            modelBuilder.Entity("FrontEnd.Models.Pago", b =>
                {
                    b.HasOne("FrontEnd.Models.Cita", "IdCitaNavigation")
                        .WithMany("Pagos")
                        .HasForeignKey("IdCita")
                        .HasConstraintName("FK__Pago__id_paquete__4CA06362");

                    b.Navigation("IdCitaNavigation");
                });

            modelBuilder.Entity("FrontEnd.Models.Personal", b =>
                {
                    b.HasOne("FrontEnd.Models.Clinica", "IdClinicaNavigation")
                        .WithMany("PersonalCollection")
                        .HasForeignKey("IdClinica")
                        .HasConstraintName("FK__Usuario__id_sucu__44FF419A");

                    b.HasOne("FrontEnd.Models.Rol", "IdRolNavigation")
                        .WithMany("PersonalCollection")
                        .HasForeignKey("IdRol")
                        .HasConstraintName("FK__Usuario__id_rol__440B1D61");

                    b.Navigation("IdClinicaNavigation");

                    b.Navigation("IdRolNavigation");
                });

            modelBuilder.Entity("FrontEnd.Models.Propietario", b =>
                {
                    b.HasOne("FrontEnd.Models.Rol", "Rol")
                        .WithMany("Propietarios")
                        .HasForeignKey("IdRol")
                        .HasConstraintName("FK__Cliente__id_rol__412EB0B6");

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("FrontEnd.Models.Cita", b =>
                {
                    b.Navigation("CitaPersonalClinicas");

                    b.Navigation("HistorialCitas");

                    b.Navigation("Pagos");
                });

            modelBuilder.Entity("FrontEnd.Models.Clinica", b =>
                {
                    b.Navigation("CitaIdClinicaDestinoNavigations");

                    b.Navigation("CitaIdClinicaOrigenNavigations");

                    b.Navigation("CitaPersonalClinicas");

                    b.Navigation("PersonalCollection");
                });

            modelBuilder.Entity("FrontEnd.Models.Personal", b =>
                {
                    b.Navigation("CitaIdVeterinarioAsignadoNavigations");

                    b.Navigation("CitaPersonalClinicas");

                    b.Navigation("HistorialCitas");

                    b.Navigation("Notificaciones");
                });

            modelBuilder.Entity("FrontEnd.Models.Propietario", b =>
                {
                    b.Navigation("Citas");
                });

            modelBuilder.Entity("FrontEnd.Models.Rol", b =>
                {
                    b.Navigation("PersonalCollection");

                    b.Navigation("Propietarios");
                });
#pragma warning restore 612, 618
        }
    }
}
