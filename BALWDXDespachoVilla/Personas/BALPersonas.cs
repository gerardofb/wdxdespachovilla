using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALWDXDespachoVilla.Personas;
using DTOWDXDespachoVilla.Personas;

namespace BALWDXDespachoVilla.Personas
{
    public class BALPersonas
    {
        public Tuple<bool, string> UpdatePersona(List<dtoPersonas> Personas)
        {
            return new DALPersonas().UpdatePersona(Personas);
        }
        public List<dtoPersonas> GetPersonasDirectorio(List<dtoPersonas> Personas = null)
        {
            var _result = new DALPersonas().GetPersonasDirectorio(Personas);

            foreach (dtoPersonas Persona in _result)
            {
                if (String.IsNullOrEmpty(Persona.ApellidoPaterno))
                {
                    Persona.NombreCompleto = Persona.Nombres;
                }
            }
            List<dtoPersonas> _resultado = new List<dtoPersonas>();
            var Agrupados = _result.GroupBy(x => x.Telefono);
            foreach(var item in Agrupados)
            {
                _resultado.Add(_result.FirstOrDefault(k => k.Telefono == item.Key));
            }
            return _resultado.OrderBy(x=> x.Nombres).ThenBy(x=> x.ApellidoPaterno).ToList();
        }
    }
}
