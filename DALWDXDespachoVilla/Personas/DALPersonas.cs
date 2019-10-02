using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DTOWDXDespachoVilla.Personas;
using DTOWDXDespachoVilla.Constantes;
using System.Configuration;
using System.Data.SqlClient;

namespace DALWDXDespachoVilla.Personas
{
    public class DALPersonas
    {
        public List<dtoPersonas> GetPersonasDirectorio(List<dtoPersonas> Personas = null)
        {
            List<dtoPersonas> _result = new List<dtoPersonas>();
            try
            {
                DataTable DtPersona = new DataTable();
                DtPersona.Columns.Add("IdPersona", typeof(int));
                DtPersona.Columns.Add("Nombres", typeof(string));
                DtPersona.Columns.Add("ApellidoPaterno", typeof(string));
                DtPersona.Columns.Add("ApellidoMaterno", typeof(string));
                DtPersona.Columns.Add("Calle", typeof(string));
                DtPersona.Columns.Add("NumeroExterior", typeof(string));
                DtPersona.Columns.Add("NumeroInterior", typeof(string));
                DtPersona.Columns.Add("Colonia", typeof(string));
                DtPersona.Columns.Add("Localidad", typeof(string));
                DtPersona.Columns.Add("CodigoPostal", typeof(int));
                DtPersona.Columns.Add("EntidadFederativa", typeof(string));
                DtPersona.Columns.Add("Telefono", typeof(string));
                DtPersona.Columns.Add("Email", typeof(string));

                if (Personas != null)
                {
                    foreach (dtoPersonas Persona in Personas)
                    {
                        int CodigoPostal = !String.IsNullOrEmpty(Persona.CodigoPostal) ? Convert.ToInt32(Persona.CodigoPostal) : 0;
                        object ApMaterno = !String.IsNullOrEmpty(Persona.ApellidoMaterno) ? (object)Persona.ApellidoMaterno : DBNull.Value;
                        object NumInterior = !String.IsNullOrEmpty(Persona.NumeroInterior) ? (object)Persona.NumeroInterior : DBNull.Value;
                        object Colonia = !String.IsNullOrEmpty(Persona.Colonia) ? (object)Persona.Colonia : DBNull.Value;
                        object Localidad = !String.IsNullOrEmpty(Persona.Localidad) ? (object)Persona.Localidad : DBNull.Value;
                        object EntidadFederativa = !String.IsNullOrEmpty(Persona.EntidadFederativa) ? (object)Persona.EntidadFederativa : DBNull.Value;
                        object Email = !String.IsNullOrEmpty(Persona.Email) ? (object)Persona.Email : DBNull.Value;
                        DataRow row = DtPersona.NewRow();
                        row["IdPersona"] = Persona.IdPersona;
                        row["Nombres"] = Persona.Nombres;
                        row["ApellidoPaterno"] = Persona.ApellidoPaterno;
                        row["ApellidoMaterno"] = ApMaterno;
                        row["NumeroExterior"] = Persona.NumeroExterior;
                        row["NumeroInterior"] = NumInterior;
                        row["Calle"] = Persona.Calle;
                        row["Colonia"] = Colonia;
                        row["Localidad"] = Localidad;
                        row["CodigoPostal"] = CodigoPostal;
                        row["EntidadFederativa"] = EntidadFederativa;
                        row["Telefono"] = Persona.Telefono;
                        row["Email"] = Email;
                        DtPersona.Rows.Add(row);
                    }
                }
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = ConstantesPersonas.PROCEDURE_GETPERSONAS;
                    SqlParameter ParamPersona = Cmd.Parameters.AddWithValue("@Personas", DtPersona);
                    SqlParameter ParamTodos = new SqlParameter("@Todos", SqlDbType.Bit,1);
                    ParamTodos.Value = Personas == null ? 0 : 1;
                    Cmd.Parameters.Add(ParamTodos);
                    ParamPersona.SqlDbType = SqlDbType.Structured;
                    ParamPersona.TypeName = "dbo.tipoPersoaWDXDespachoVilla";
                    IDataReader Reader = Cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        _result.Add(new dtoPersonas
                        {
                            Nombres = Reader["Nombres"].ToString(),
                            IdPersona = Convert.ToInt32(Reader["Id_Persona"]),
                            ApellidoPaterno = Reader["ApPaterno"].ToString(),
                            ApellidoMaterno = Reader["ApMaterno"].ToString(),
                            Calle = Reader["Calle"].ToString(),
                            CodigoPostal = Reader["CP"].ToString(),
                            EntidadFederativa = Reader["EntidadFederativa"].ToString(),
                            Localidad = Reader["Localidad"].ToString(),
                            Colonia = Reader["Colonia"].ToString(),
                            NumeroExterior = Reader["NumExterior"].ToString(),
                            NumeroInterior = Reader["NumInterior"].ToString(),
                            Telefono = Reader["Telefono"].ToString()

                        });
                        
                    }
                    Reader.Close();
                    Conn.Close();
                    if (_result.Count == 0)
                    {
                        _result = null;
                    }
                }

            }
            catch(Exception ex)
            {
                _result = null;
            }
            return _result;
        }

        public Tuple<bool, string> UpdatePersona(List<dtoPersonas> Personas)
        {
            Tuple<bool, string> _result = new Tuple<bool, string>(false, ConstantesComunes.ERROR_GENERICO);
            try
            {
                DataTable DtPersona = new DataTable();
                DtPersona.Columns.Add("IdPersona", typeof(int));
                DtPersona.Columns.Add("Nombres", typeof(string));
                DtPersona.Columns.Add("ApellidoPaterno", typeof(string));
                DtPersona.Columns.Add("ApellidoMaterno", typeof(string));
                DtPersona.Columns.Add("Calle", typeof(string));
                DtPersona.Columns.Add("NumeroExterior", typeof(string));
                DtPersona.Columns.Add("NumeroInterior", typeof(string));
                DtPersona.Columns.Add("Colonia", typeof(string));
                DtPersona.Columns.Add("Localidad", typeof(string));
                DtPersona.Columns.Add("CodigoPostal", typeof(int));
                DtPersona.Columns.Add("EntidadFederativa", typeof(string));
                DtPersona.Columns.Add("Telefono", typeof(string));
                DtPersona.Columns.Add("Email", typeof(string));

                int IdentificadorPersona = 0;
                foreach (dtoPersonas Persona in Personas)
                {
                    int CodigoPostal = !String.IsNullOrEmpty(Persona.CodigoPostal) ? Convert.ToInt32(Persona.CodigoPostal) : 0;
                    object ApPaterno = !String.IsNullOrEmpty(Persona.ApellidoPaterno) ? (object)Persona.ApellidoPaterno : DBNull.Value;
                    object ApMaterno = !String.IsNullOrEmpty(Persona.ApellidoMaterno) ? (object)Persona.ApellidoMaterno : DBNull.Value;
                    object NumInterior = !String.IsNullOrEmpty(Persona.NumeroInterior) ? (object)Persona.NumeroInterior : DBNull.Value;
                    object Colonia = !String.IsNullOrEmpty(Persona.Colonia) ? (object)Persona.Colonia : DBNull.Value;
                    object Localidad = !String.IsNullOrEmpty(Persona.Localidad) ? (object)Persona.Localidad : DBNull.Value;
                    object EntidadFederativa = !String.IsNullOrEmpty(Persona.EntidadFederativa) ? (object)Persona.EntidadFederativa : DBNull.Value;
                    object Email = !String.IsNullOrEmpty(Persona.Email) ? (object)Persona.Email : DBNull.Value;
                    DataRow row = DtPersona.NewRow();
                    row["IdPersona"] = Persona.IdPersona == 0 ? ++IdentificadorPersona : Persona.IdPersona;
                    row["Nombres"] = String.IsNullOrEmpty(Persona.Nombres) ? Persona.NombreCompleto : Persona.Nombres;
                    row["ApellidoPaterno"] = ApPaterno;
                    row["ApellidoMaterno"] = ApMaterno;
                    row["NumeroExterior"] = Persona.NumeroExterior;
                    row["NumeroInterior"] = NumInterior;
                    row["Calle"] = !String.IsNullOrEmpty(Persona.Calle) ? (object)Persona.Calle : DBNull.Value;
                    row["Colonia"] = Colonia;
                    row["Localidad"] = Localidad;
                    row["CodigoPostal"] = CodigoPostal;
                    row["EntidadFederativa"] = EntidadFederativa;
                    row["Telefono"] = Persona.Telefono;
                    row["Email"] = Email;
                    DtPersona.Rows.Add(row);
                }
                string conexion = ConfigurationManager.ConnectionStrings["WDXAdminConnection"].ToString();
                using (SqlConnection Conn = new SqlConnection(conexion))
                {
                    Conn.Open();
                    SqlCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = ConstantesPersonas.PROCEDURE_INSERTPERSONA;
                    SqlParameter ParamPersona = Cmd.Parameters.AddWithValue("@Personas", DtPersona);
                    SqlParameter ParamResult = new SqlParameter("@Result", SqlDbType.VarChar, -1);
                    ParamResult.Direction = ParameterDirection.Output;
                    Cmd.Parameters.Add(ParamResult);
                    ParamPersona.SqlDbType = SqlDbType.Structured;
                    ParamPersona.TypeName = "dbo.tipoPersoaWDXDespachoVilla";
                    Cmd.ExecuteNonQuery();
                    if (Cmd.Parameters[1].Value.ToString() == "")
                    {
                        _result = new Tuple<bool, string>(true, ConstantesComunes.EXITO_GENERICO);
                    }
                    Conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return _result;
        }
    }
}
