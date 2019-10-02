using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOWDXDespachoVilla.Constantes;
using DTOWDXDespachoVilla.Asuntos;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace DALWDXDespachoVilla.Asuntos
{
    public class DALAsuntos
    {
        public List<dtoAsuntos> GetAllAsuntosArchivos(bool Todos = false)
        {
            List<dtoAsuntos> _result = new List<dtoAsuntos>();
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandText = ConstantesAsuntos.PROCEDUER_SP_CONSULTATODOSASUNTOSARCHIVOS;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Todos", Todos ? 0 : 1);
                    IDataReader Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        _result.Add(new dtoAsuntos
                        {
                            Email = Reader["UserName"].ToString(),
                            NombreArchivo = Reader["NombreArchivo"].ToString(),
                            Archivo = Reader["Archivo"] != DBNull.Value ? (byte[])Reader["Archivo"] : null,
                            GuidArchivo = Reader["GuidArchivo"].ToString(),
                            IdAsunto = Convert.ToInt32(Reader["Id_Asunto"]),
                            Asunto = Reader["Asunto"].ToString(),
                            Tema = Reader["Tema"].ToString(),
                            FechaCreacion = Convert.ToDateTime(Reader["FechaCreacion"]),
                            FechaAprobacion = Reader["FechaAprobacion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(Reader["FechaAprobacion"]) : null,
                            FechaArchivo = Reader["FechaArchivo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(Reader["FechaArchivo"]) : null,
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }

        public List<dtoAsuntos> GetAsuntosArchivos(dtoAsuntos Asunto, bool Todos = false)
        {
            List<dtoAsuntos> _result = new List<dtoAsuntos>();
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandText = ConstantesAsuntos.PROCEDURE_SP_CONSULTAASUNTOS;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Usuario", Asunto.Email);
                    Cmd.Parameters.AddWithValue("@Todos", Todos ? 0 : 1);
                    IDataReader Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        _result.Add(new dtoAsuntos
                        {
                            Email = Reader["UserName"].ToString(),
                            NombreArchivo = Reader["NombreArchivo"].ToString(),
                            Archivo = Reader["Archivo"] != DBNull.Value ? (byte[])Reader["Archivo"] : null,
                            GuidArchivo = Reader["GuidArchivo"].ToString(),
                            IdAsunto = Convert.ToInt32(Reader["Id_Asunto"]),
                            Asunto = Reader["Asunto"].ToString(),
                            Tema = Reader["Tema"].ToString(),
                            FechaCreacion = Convert.ToDateTime(Reader["FechaCreacion"]),
                            FechaAprobacion = Reader["FechaAprobacion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(Reader["FechaAprobacion"]) : null,
                            FechaArchivo = Reader["FechaArchivo"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(Reader["FechaArchivo"]) : null,
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }

        public Tuple<bool, byte[]> DownloadArchivoAsunto(dtoAsuntos Asunto, bool Todos = true)
        {
            Tuple<bool, byte[]> _result = new Tuple<bool, byte[]>(false, new byte[0]);
            try
            {
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandText = ConstantesAsuntos.PROCEDURE_SP_DESCARGARARCHIVO;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@GuidArchivo", Asunto.GuidArchivo);
                    Cmd.Parameters.AddWithValue("@Todos", Todos ? DBNull.Value : (object)false);
                    var myReader = Cmd.ExecuteReader();
                    byte[] Archivo = null;
                    while (myReader.Read())
                    {
                        Archivo = (byte[])myReader["Archivo"];
                    }
                    _result = new Tuple<bool, byte[]>(true, Archivo);
                }

            }
            catch (Exception ex)
            {

            }
            return _result;
        }

        public Tuple<bool, string> UpdateAsuntos(dtoAsuntos Asunto, bool Actualizar = false)
        {
            Tuple<bool, string> _result = new Tuple<bool, string>(false, ConstantesComunes.ERROR_GENERICO);
            try
            {

                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandText = ConstantesAsuntos.PROCEDURE_SP_UPDATEASUNTOS;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Usuario", Asunto.Email);
                    Cmd.Parameters.AddWithValue("@Asunto", Asunto.Asunto.Trim());
                    Cmd.Parameters.AddWithValue("@HashAsunto", Asunto.HashAsunto);
                    Cmd.Parameters.AddWithValue("@Accion", Actualizar ? 1 : 0);
                    SqlParameter ParamResult = new SqlParameter("@Result", SqlDbType.VarChar, -1);
                    ParamResult.Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add(ParamResult);
                    Cmd.ExecuteNonQuery();
                    if (Cmd.Parameters[4].Value.ToString() != String.Empty)
                    {
                        _result = new Tuple<bool, string>(false, Cmd.Parameters[4].Value.ToString());
                    }
                    _result = new Tuple<bool, string>(true, ConstantesComunes.EXITO_GENERICO);
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }

        public Tuple<bool, string> UpdateArchivosAsuntos(List<dtoAsuntos> Archivos, bool Desactivar = false)
        {
            Tuple<bool, string> _result = new Tuple<bool, string>(false, ConstantesComunes.ERROR_GENERICO);
            try
            {
                DataTable DtArchivos = new DataTable();
                DtArchivos.Columns.Add("IdUsuario", typeof(string));
                DtArchivos.Columns.Add("IdAsunto", typeof(int));
                DtArchivos.Columns.Add("GuidArchivo", typeof(System.Guid));
                DtArchivos.Columns.Add("Archivo", typeof(SqlBinary));
                DtArchivos.Columns.Add("Extension", typeof(string));
                DtArchivos.Columns.Add("NombreArchivo", typeof(string));
                DtArchivos.Columns.Add("Activo", typeof(bool));
                foreach (dtoAsuntos asunto in Archivos)
                {
                    DataRow row = DtArchivos.NewRow();
                    row["IdUsuario"] = asunto.IdUsuario;
                    row["IdAsunto"] = asunto.IdAsunto;
                    row["GuidArchivo"] = !String.IsNullOrEmpty(asunto.GuidArchivo) ? (object)Guid.Parse(asunto.GuidArchivo) : DBNull.Value;
                    row["Archivo"] = asunto.Archivo;
                    row["Extension"] = asunto.NombreArchivo.Substring(asunto.NombreArchivo.LastIndexOf("."), asunto.NombreArchivo.Length - asunto.NombreArchivo.LastIndexOf("."));
                    row["NombreArchivo"] = asunto.NombreArchivo.Substring(0, asunto.NombreArchivo.LastIndexOf("."));
                    row["Activo"] = Desactivar ? false : true;
                    DtArchivos.Rows.Add(row);
                }
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandText = ConstantesAsuntos.PROCEDURE_SP_UPDATEARCHIVOS;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter ParamArchivos = new SqlParameter("@Archivos", SqlDbType.Structured);
                    SqlParameter ParamResult = new SqlParameter("@Result", SqlDbType.VarChar, -1);
                    ParamArchivos.TypeName = "dbo.tipoArchivoAsuntoWDXDespachoVilla";
                    ParamArchivos.Value = DtArchivos;
                    ParamResult.Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add(ParamArchivos);
                    Cmd.Parameters.Add(ParamResult);
                    Cmd.ExecuteNonQuery();
                    if (!String.IsNullOrEmpty(Cmd.Parameters[1].Value.ToString()))
                    {
                        throw new Exception(Cmd.Parameters[1].Value.ToString());
                    }
                }
                _result = new Tuple<bool, string>(true, ConstantesComunes.EXITOWEB_GENERIO);
            }
            catch (Exception ex)
            {
                _result = new Tuple<bool, string>(false, ex.Message.ToString());
            }
            return _result;
        }
    }
}
