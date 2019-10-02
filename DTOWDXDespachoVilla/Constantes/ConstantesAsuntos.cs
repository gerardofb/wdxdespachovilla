using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOWDXDespachoVilla.Constantes
{
    public class ConstantesAsuntos
    {
        public const string PROCEDURE_SP_UPDATEASUNTOS = "[dbo].[SP_UPDATEASUNTOWDXVilla]";
        public const string PROCEDUER_SP_CONSULTATODOSASUNTOSARCHIVOS = "[dbo].[SP_CONSULTA_TODOSASUNTOSARCHIVOSWDXVilla]";
        public const string PROCEDURE_SP_CONSULTAASUNTOS = "[dbo].[SP_CONSULTA_ASUNTOSARCHIVOSWDXVilla]";
        public const string PROCEDURE_SP_UPDATEARCHIVOS = "[dbo].[SP_UPDATEARCHIVOASUNTOWDXVilla]";
        public const string PROCEDURE_SP_DESCARGARARCHIVO = "[dbo].[SP_DESCARGAARCHIVOASUNTOWDXVilla]";
        public const string VERIFICAR_ASUNTO = " En breve verificaremos la procedencia de su asunto y podrá agregar documentos desde esta misma página.";
    }
}
