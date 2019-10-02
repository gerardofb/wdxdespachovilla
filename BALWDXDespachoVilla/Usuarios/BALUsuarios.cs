using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOWDXDespachoVilla.Usuarios;
using DALWDXDespachoVilla.Usuarios;

namespace BALWDXDespachoVilla.Usuarios
{
    public class BALUsuarios
    {
        public Tuple<bool, List<dtoPreguntasAcceso>> GetPreguntasAccesoAplicativo(bool Activo = false)
        {
            return new DALUsuarios().GetPreguntasAccesoAplicativo(Activo);
        }
        public Tuple<bool, string> UpdateUsuarioAplicativo(dtoUsuarioAplicativo Usuario)
        {
            return new DALUsuarios().UpdateUsuarioAplicativo(Usuario);
        }
        public Tuple<bool, string> UpdatePreguntasUsuarioAplicativo(List<dtoPreguntasUsuarios> PreguntasUsuario)
        {
            return new DALUsuarios().UpdatePreguntasUsuarioAplicativo(PreguntasUsuario);
        }
    }
}
