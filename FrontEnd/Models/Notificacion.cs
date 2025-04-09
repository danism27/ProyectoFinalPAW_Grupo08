using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("Notificacion")]
public partial class Notificacion
{
    [Key]
    [Column("id_notificacion")]
    public int IdNotificacion { get; set; }

    [Column("id_personal")]
    public int? IdPersonal { get; set; }

    [Column("mensaje")]
    public string? Mensaje { get; set; }

    [Column("fecha_envio")]
    public DateTime? FechaEnvio { get; set; }

    [Column("leida")]
    public bool? Leida { get; set; }

    [ForeignKey("IdPersonal")]
    [InverseProperty("Notificaciones")]
    public virtual Personal? IdPersonalNavigation { get; set; }
}
