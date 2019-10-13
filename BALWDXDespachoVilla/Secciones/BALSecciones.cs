using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOWDXDespachoVilla.Secciones;
using DALWDXDespachoVilla.Secciones;

namespace BALWDXDespachoVilla.Secciones
{
    public class BALSecciones
    {
        public Tuple<bool, string> UpdateSeccion(dtoSeccion Seccion)
        {
            return new DALSecciones().UpdateSeccion(Seccion);
        }
        public Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>> GetCatalogoSecciones(int Opcion)
        {
            return new DALSecciones().GetCatalogoSecciones(Opcion);
        }
    }
}
