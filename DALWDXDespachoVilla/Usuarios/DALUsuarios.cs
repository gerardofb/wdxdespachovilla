using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DTOWDXDespachoVilla.Usuarios;
using DTOWDXDespachoVilla.Constantes;
using System.Data.SqlClient;
using System.Configuration;

namespace DALWDXDespachoVilla.Usuarios
{
    public class DALUsuarios
    {
        public Tuple<bool, string> UpdatePreguntasUsuarioAplicativo(List<dtoPreguntasUsuarios> PreguntasUsuario)
        {
            Tuple<bool, string> _resultado = new Tuple<bool, string>(false, ConstantesComunes.ERROR_GENERICO);
            try
            {
                DataTable DtPreguntas = new DataTable();
                DtPreguntas.Columns.Add("IdUsuario", typeof(int));
                DtPreguntas.Columns.Add("IdPregunta", typeof(int));
                DtPreguntas.Columns.Add("Respuesta", typeof(string));

                if (PreguntasUsuario != null)
                {
                    foreach (dtoPreguntasUsuarios Pregunta in PreguntasUsuario)
                    {
                        DataRow row = DtPreguntas.NewRow();
                        row["IdUsuario"] = Pregunta.Usuario.IdUsuario;
                        row["IdPregunta"] = Pregunta.Pregunta.IdPregunta;
                        row["Respuesta"] = Pregunta.Pregunta.Respuesta;
                        DtPreguntas.Rows.Add(row);
                    }
                }
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = ConstantesUsuarios.PROCEDURE_UPDATEPREGUNTASUSER;
                    SqlParameter ParamPregunta = Cmd.Parameters.AddWithValue("@UsuariosPreguntas", DtPreguntas);
                    SqlParameter ParamResult = new SqlParameter("@Result", SqlDbType.VarChar, -1);
                    ParamResult.Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add(ParamResult);
                    ParamPregunta.SqlDbType = SqlDbType.Structured;
                    ParamPregunta.TypeName = "dbo.tipoPreguntasUsuariosWDXDespachoVilla";
                    Cmd.ExecuteNonQuery();
                    if (String.IsNullOrEmpty(Cmd.Parameters[1].Value.ToString()))
                    {
                        _resultado = new Tuple<bool, string>(true, ConstantesComunes.EXITO_GENERICO); 
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _resultado;
        }
        /// <summary>
        /// Actualiza un usuario del aplicativo de escritorio, devuelve una tupla con el resultado de la 
        /// operación y el Identificador del Usuario insertado
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public Tuple<bool, string> UpdateUsuarioAplicativo(dtoUsuarioAplicativo Usuario)
        {
            Tuple<bool, string> _resultado = new Tuple<bool, string>(false, ConstantesComunes.ERROR_GENERICO);
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = ConstantesUsuarios.PROCEDURE_UPDATEUSER;
                    Cmd.Parameters.AddWithValue("@IdUsuario", Usuario.IdUsuario);
                    Cmd.Parameters.AddWithValue("@NombreUsuario", Usuario.NombreUsuario);
                    Cmd.Parameters.AddWithValue("@HashAcceso", Usuario.HashAcceso);
                    Cmd.Parameters.AddWithValue("@Activo", Usuario.Activo);
                    SqlParameter ParamResult = new SqlParameter("@Result", SqlDbType.VarChar, -1);
                    ParamResult.Direction = ParameterDirection.Output;
                    SqlParameter ParamUsuario = new SqlParameter("@ResultIdUsuario", SqlDbType.VarChar,-1);
                    ParamUsuario.Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add(ParamResult);
                    Cmd.Parameters.Add(ParamUsuario);
                    Cmd.ExecuteNonQuery();
                    if (String.IsNullOrEmpty(Cmd.Parameters[4].Value.ToString()))
                    {
                        _resultado = new Tuple<bool, string>(true, ParamUsuario.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _resultado;
        }

        public Tuple<bool, List<dtoPreguntasAcceso>> GetPreguntasAccesoAplicativo(bool Activo = false)
        {
            List<dtoPreguntasAcceso> Listado = new List<dtoPreguntasAcceso>();
            Tuple<bool, List<dtoPreguntasAcceso>> _result = new Tuple<bool, List<dtoPreguntasAcceso>>(false, Listado);
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = CommandType.Text;
                    string TextoComando = @"SELECT IdPregunta, Pregunta FROM WDXPreguntasAccesoAplicativo";
                    if (Activo)
                        TextoComando = TextoComando + " WHERE Activo = 1";
                    Cmd.CommandText = TextoComando;
                    IDataReader Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        Listado.Add(new dtoPreguntasAcceso
                        {
                            IdPregunta = Convert.ToInt32(Reader["IdPregunta"]),
                            Pregunta = Reader["Pregunta"].ToString()
                        });
                    }
                    if (Listado.Count > 0)
                    {
                        return new Tuple<bool, List<dtoPreguntasAcceso>>(true, Listado);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }
    }
}
