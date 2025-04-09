using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

public partial class Propietario
{
    [Key]
    [Column("id_propietario")]
    public int IdPropietario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
    [Column("apellido")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
    [Column("correo")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [Column("contrasenna")]
    public string Contrasenna { get; set; } = null!;

    [Phone(ErrorMessage = "Formato de teléfono inválido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("id_rol")]
    public int? IdRol { get; set; }

    [ForeignKey("IdRol")]
    public virtual Rol? Rol { get; set; }

    [InverseProperty("IdPropietarioNavigation")]
    public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
}
