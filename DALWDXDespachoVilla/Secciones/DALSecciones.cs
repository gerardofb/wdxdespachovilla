using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOWDXDespachoVilla.Secciones;
using DTOWDXDespachoVilla.Constantes;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace DALWDXDespachoVilla.Secciones
{
    public class DALSecciones
    {
        public Tuple<bool, string> UpdateSeccion(dtoSeccion Seccion)
        {
            Tuple<bool, string> _result = new Tuple<bool, string>(false, ConstantesComunes.ERROR_GENERICO);
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.CommandText = ConstantesSecciones.SP_ACTUALIZA_SECCIONES;
                    Cmd.Parameters.AddWithValue("@IdSeccion", Seccion.IdSeccion);
                    Cmd.Parameters.Add("@TextoSeccion", SqlDbType.VarChar, 4000).Value = Seccion.Texto;
                    Cmd.Parameters.Add("@Result", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output;
                    Cmd.ExecuteNonQuery();
                    if (String.IsNullOrEmpty(Cmd.Parameters[2].Value.ToString()))
                    {
                        _result = new Tuple<bool, string>(true, ConstantesComunes.EXITO_GENERICO);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }
        public Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>> GetCatalogoSecciones(int Opcion)
        {
            List<dtoCatSecciones> _resultado = new List<dtoCatSecciones>();
            List<dtoSeccion> _resultadoSeccion = new List<dtoSeccion>();
            Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>> _result = new Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>>(ConstantesComunes.ERROR_GENERICO, _resultado, _resultadoSeccion);
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.CommandText = ConstantesSecciones.SP_CONSULTA_SECCIONES;
                    Cmd.Parameters.AddWithValue("@Opcion", Opcion);
                    IDataReader reader = Cmd.ExecuteReader();
                    if (Opcion == 1)
                    {
                        while (reader.Read())
                        {
                            _resultado.Add(new dtoCatSecciones
                            {
                                IdSeccion = Convert.ToInt32(reader["IdSeccion"]),
                                Seccion = reader["Seccion"].ToString()
                            });
                        }
                    }
                    else if (Opcion == 2)
                    {
                        while (reader.Read())
                        {
                            _resultadoSeccion.Add(new dtoSeccion
                            {
                                IdSeccion = Convert.ToInt32(reader["IdSeccion"]),
                                Texto = reader["TextoSeccion"].ToString()
                            });
                        }
                    }
                    _result = new Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>>(String.Empty, _resultado, _resultadoSeccion);
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }
    }
}
