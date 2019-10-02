using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DTOWDXDespachoVilla.Asuntos
{
    public class ArchivoDriveAsunto
    {
        public string Asunto { get; set; }
        public string NombreArchivo { get; set; }
        public string GuidArchivo { get; set; }
    }
    public class dtoAsuntos
    {
        public List<dtoAsuntos> ArchivosAsuntos { get; set; }
        [StringLength(128, ErrorMessage ="El identificador del usuario no es válido", MinimumLength =128)]
        public string IdUsuario { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(300, ErrorMessage ="El texto del asunto es demasiado largo, debe ser igual o menor a 300 caracteres")]
        [Display(Name ="Asunto")]
        public string Asunto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        [StringLength(64, ErrorMessage ="No se computó el Hash adecuadamente, intente más tarde", MinimumLength =64)]
        public string HashAsunto { get; set; }
        public byte[] Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string GuidArchivo { get; set; }
        public int IdAsunto { get; set; }
        public string Tema { get; set; }
        public DateTime? FechaArchivo { get; set; }
        public bool Drive { get; set; }
        public bool DriveCompartido { get; set; }
    }
}
