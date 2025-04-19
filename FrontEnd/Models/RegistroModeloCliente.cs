using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class RegistroModeloCliente
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        [StringLength(25, ErrorMessage = "El primer apellido no puede exceder los 25 caracteres.")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [StringLength(25, ErrorMessage = "El segundo apellido no puede exceder los 25 caracteres.")]
        [Display(Name = "Segundo Apellido")]
        public string? SegundoApellido { get; set; }

        [Required(ErrorMessage = "El teléfono principal es obligatorio.")]
        [Phone(ErrorMessage = "Formato de teléfono inválido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono Principal")]
        public string TelefonoPrincipal { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de teléfono inválido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono Secundario")]
        public string? TelefonoSecundario { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 255 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmarContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; } = 1;

        [StringLength(50, ErrorMessage = "La provincia no puede exceder los 50 caracteres.")]
        [Display(Name = "Provincia")]
        public string? Provincia { get; set; }

        [StringLength(50, ErrorMessage = "El cantón no puede exceder los 50 caracteres.")]
        [Display(Name = "Cantón")]
        public string? Canton { get; set; }

        [StringLength(50, ErrorMessage = "El distrito no puede exceder los 50 caracteres.")]
        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }

        [StringLength(10, ErrorMessage = "El código postal no puede exceder los 10 caracteres.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }
    }
}
