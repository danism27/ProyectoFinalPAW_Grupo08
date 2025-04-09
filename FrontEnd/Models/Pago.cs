using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("Pago")]
public partial class Pago
{
    [Key]
    [Column("id_pago")]
    public int IdPago { get; set; }

    [Column("id_cita")]
    public int? IdCita { get; set; }

    [Column("monto", TypeName = "decimal(10, 2)")]
    public decimal? Monto { get; set; }

    [Column("fecha_pago")]
    public DateTime? FechaPago { get; set; }

    [StringLength(50)]
    [Column("metodo_pago")]
    public string? MetodoPago { get; set; }

    [ForeignKey("IdCita")]
    [InverseProperty("Pagos")]
    public virtual Cita? IdCitaNavigation { get; set; }
}
