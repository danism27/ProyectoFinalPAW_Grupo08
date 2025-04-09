using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("Cita")]
public partial class Cita
{
    [Key]
    [Column("id_cita")]
    public int IdCita { get; set; }

    [Column("id_propietario")]
    public int? IdPropietario { get; set; }

    [Column("id_clinica_origen")]
    public int? IdClinicaOrigen { get; set; }

    [Column("id_clinica_destino")]
    public int? IdClinicaDestino { get; set; }

    [StringLength(500)]
    [Column("motivo_consulta")]
    public string? MotivoConsulta { get; set; }

    [Column("peso_mascota", TypeName = "decimal(10, 2)")]
    public decimal? PesoMascota { get; set; }

    [StringLength(100)]
    [Column("detalles_mascota")]
    public string? DetallesMascota { get; set; }

    [StringLength(50)]
    [Column("estado_cita")]
    public string? EstadoCita { get; set; }

    [Column("fecha_creacion")]
    public DateTime? FechaCreacion { get; set; }

    [Column("fecha_hora_cita")]
    public DateTime? FechaHoraCita { get; set; }

    [Column("id_veterinario_asignado")]
    public int? IdVeterinarioAsignado { get; set; }

    [ForeignKey("IdClinicaDestino")]
    [InverseProperty("CitaIdClinicaDestinoNavigations")]
    public virtual Clinica? IdClinicaDestinoNavigation { get; set; }

    [ForeignKey("IdClinicaOrigen")]
    [InverseProperty("CitaIdClinicaOrigenNavigations")]
    public virtual Clinica? IdClinicaOrigenNavigation { get; set; }

    [ForeignKey("IdPropietario")]
    [InverseProperty("Citas")]
    public virtual Propietario? IdPropietarioNavigation { get; set; }

    [ForeignKey("IdVeterinarioAsignado")]
    [InverseProperty("CitaIdVeterinarioAsignadoNavigations")]
    public virtual Personal? IdVeterinarioAsignadoNavigation { get; set; }

    [InverseProperty("IdCitaNavigation")]
    public virtual ICollection<CitaPersonalClinica> CitaPersonalClinicas { get; set; } = new List<CitaPersonalClinica>();

    [InverseProperty("IdCitaNavigation")]
    public virtual ICollection<HistorialCita> HistorialCitas { get; set; } = new List<HistorialCita>();

    [InverseProperty("IdCitaNavigation")]
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
