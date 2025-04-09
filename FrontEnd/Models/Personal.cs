using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("Personal")]
public partial class Personal
{
    [Key]
    [Column("id_personal")]
    public int IdPersonal { get; set; }

    [StringLength(50)]
    [Column("nombre")]
    public string? Nombre { get; set; }

    [StringLength(50)]
    [Column("apellido")]
    public string? Apellido { get; set; }

    [StringLength(100)]
    [Column("correo")]
    public string? Correo { get; set; }

    [StringLength(255)]
    [Column("contrasenna")]
    public string? Contrasenna { get; set; }

    [StringLength(20)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [StringLength(255)]
    [Column("foto_perfil")] // <-- ¡NUEVA PROPIEDAD!
    public string? FotoPerfil { get; set; }

    [Column("id_rol")]
    public int? IdRol { get; set; }

    [Column("id_clinica")]
    public int? IdClinica { get; set; }

    [ForeignKey("IdClinica")]
    [InverseProperty("PersonalCollection")]
    public virtual Clinica? IdClinicaNavigation { get; set; }

    [ForeignKey("IdRol")]
    [InverseProperty("PersonalCollection")]
    public virtual Rol? IdRolNavigation { get; set; }

    [InverseProperty("IdPersonalModificacionNavigation")]
    public virtual ICollection<HistorialCita> HistorialCitas { get; set; } = new List<HistorialCita>();

    [InverseProperty("IdPersonalNavigation")]
    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    [InverseProperty("IdVeterinarioAsignadoNavigation")]
    public virtual ICollection<Cita> CitaIdVeterinarioAsignadoNavigations { get; set; } = new List<Cita>();

    [InverseProperty("IdPersonalNavigation")]
    public virtual ICollection<CitaPersonalClinica> CitaPersonalClinicas { get; set; } = new List<CitaPersonalClinica>();
}
