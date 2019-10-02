using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOWDXDespachoVilla.Usuarios
{
    public class dtoPreguntasUsuarios
    {
        public dtoPreguntasAcceso Pregunta { get; set; }
        public dtoUsuarioAplicativo Usuario { get; set; }
    }

    public class dtoPreguntasAcceso
    {
        public int IdPregunta { get; set; }
        public string Pregunta { get; set; }
    }
    public class dtoUsuarioAplicativo
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string HashAcceso { get; set; }
        public bool Activo { get; set; }
    }
}
