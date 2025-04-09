using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("CitaPersonalClinica")]
public partial class CitaPersonalClinica
{
    [Key]
    [Column("id_cita_personal_clinica")]
    public int IdCitaPersonalClinica { get; set; }

    [Column("id_cita")]
    public int? IdCita { get; set; }

    [Column("id_personal")]
    public int? IdPersonal { get; set; }

    [Column("id_clinica")]
    public int? IdClinica { get; set; }

    [Column("fecha_asignacion")]
    public DateTime? FechaAsignacion { get; set; }

    [ForeignKey("IdCita")]
    [InverseProperty("CitaPersonalClinicas")]
    public virtual Cita? IdCitaNavigation { get; set; }

    [ForeignKey("IdClinica")]
    [InverseProperty("CitaPersonalClinicas")]
    public virtual Clinica? IdClinicaNavigation { get; set; }

    [ForeignKey("IdPersonal")]
    [InverseProperty("CitaPersonalClinicas")]
    public virtual Personal? IdPersonalNavigation { get; set; }
}
