using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("Clinica")]
public partial class Clinica
{
    [Key]
    [Column("id_clinica")]
    public int IdClinica { get; set; }

    [StringLength(100)]
    [Column("nombre")]
    public string? Nombre { get; set; }

    [StringLength(255)]
    [Column("direccion")]
    public string? Direccion { get; set; }

    [StringLength(20)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [InverseProperty("IdClinicaDestinoNavigation")]
    public virtual ICollection<Cita> CitaIdClinicaDestinoNavigations { get; set; } = new List<Cita>();

    [InverseProperty("IdClinicaOrigenNavigation")]
    public virtual ICollection<Cita> CitaIdClinicaOrigenNavigations { get; set; } = new List<Cita>();

    [InverseProperty("IdClinicaNavigation")]
    public virtual ICollection<CitaPersonalClinica> CitaPersonalClinicas { get; set; } = new List<CitaPersonalClinica>();

    [InverseProperty("IdClinicaNavigation")]
    public virtual ICollection<Personal> PersonalCollection { get; set; } = new List<Personal>();
}
