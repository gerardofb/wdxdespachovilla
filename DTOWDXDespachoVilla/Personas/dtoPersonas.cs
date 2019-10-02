using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOWDXDespachoVilla.Personas
{
    public class dtoPersonas
    {
        public int IdPersona { get; set; }
        public string Nombres { get; set; }
        public string NombreCompleto { get; set; }
        public string Apellidos { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterior { get; set; }
        public string Colonia { get; set; }
        public string Localidad { get; set; }
        public string EntidadFederativa { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}
