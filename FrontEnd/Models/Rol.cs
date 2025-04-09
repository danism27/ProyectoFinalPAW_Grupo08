using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("Rol")]
public partial class Rol
{
    [Key]
    [Column("id_rol")]
    public int IdRol { get; set; }

    [StringLength(50)]
    [Column("nombre_rol")]
    public string? NombreRol { get; set; }

    [InverseProperty("Rol")]
    public virtual ICollection<Propietario> Propietarios { get; set; } = new List<Propietario>();

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<Personal> PersonalCollection { get; set; } = new List<Personal>();
}
