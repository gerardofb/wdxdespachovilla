using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALWDXDespachoVilla.Asuntos;
using DTOWDXDespachoVilla.Asuntos;

namespace BALWDXDespachoVilla.Asuntos
{
    public class BALAsuntos
    {
        public Tuple<bool, string> UpdateAsuntos(dtoAsuntos Asunto, bool Actualizar = false)
        {
            return new DALAsuntos().UpdateAsuntos(Asunto, Actualizar);
        }

        public List<dtoAsuntos> GetAllAsuntosArchivos(bool Todos = false)
        {
            return new DALAsuntos().GetAllAsuntosArchivos(Todos);
        }

        public List<dtoAsuntos> GetAsuntosArchivos(dtoAsuntos Asunto, bool Todos = false, string GuidArchivo = "")
        {
            List<dtoAsuntos> _result = new DALAsuntos().GetAsuntosArchivos(Asunto, Todos);
            return _result.Count > 0 ? GuidArchivo != String.Empty ?
                new List<dtoAsuntos> { _result.FirstOrDefault(k => k.GuidArchivo == GuidArchivo) } : _result : null;
        }

        public Tuple<bool, string> UpdateArchivosAsuntos(List<dtoAsuntos> Archivos, bool Desactivar = false)
        {
            return new DALAsuntos().UpdateArchivosAsuntos(Archivos, Desactivar);
        }

        public Tuple<bool, byte[]> DownloadArchivoAsunto(dtoAsuntos Asunto, bool Todos = true)
        {
            return new DALAsuntos().DownloadArchivoAsunto(Asunto, Todos);
        }
    }
}
