using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEnd.Models;

[Table("HistorialCita")]
public partial class HistorialCita
{
    [Key]
    [Column("id_historial")]
    public int IdHistorial { get; set; }

    [Column("id_cita")]
    public int? IdCita { get; set; }

    [Column("fecha_cambio")]
    public DateTime? FechaCambio { get; set; }

    [StringLength(50)]
    [Column("estado_anterior")]
    public string? EstadoAnterior { get; set; }

    [StringLength(50)]
    [Column("estado_nuevo")]
    public string? EstadoNuevo { get; set; }

    [StringLength(255)]
    [Column("ubicacion_actual")]
    public string? UbicacionActual { get; set; }

    [Column("id_personal_modificacion")]
    public int? IdPersonalModificacion { get; set; }

    [ForeignKey("IdCita")]
    [InverseProperty("HistorialCitas")]
    public virtual Cita? IdCitaNavigation { get; set; }

    [ForeignKey("IdPersonalModificacion")]
    [InverseProperty("HistorialCitas")]
    public virtual Personal? IdPersonalModificacionNavigation { get; set; }
}
