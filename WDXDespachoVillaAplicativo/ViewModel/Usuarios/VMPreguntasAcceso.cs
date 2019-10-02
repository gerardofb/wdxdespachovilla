using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BALWDXDespachoVilla.Usuarios;
using DTOWDXDespachoVilla.Usuarios;
using DTOWDXDespachoVilla.Constantes;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Windows;

namespace WDXDespachoVillaAplicativo.ViewModel.Usuarios
{
    public class VMPreguntasUsuario : VMBase
    {
        private string _RespuestaElegida;
        public string RespuestaElegida { get { return _RespuestaElegida; } set { _RespuestaElegida = value; OnPropertyChanged("RespuestaElegida"); } }
        public int CuentaPreguntas { get; set; }
        private bool _EsActualizacion;
        public bool EsActualizacion { get { return _EsActualizacion; } set { _EsActualizacion = value; OnPropertyChanged("EsActualizacion"); } }
        private List<VMPreguntasAcceso> _PreguntasPorUsuario;
        public List<VMPreguntasAcceso> PreguntasPorUsuario { get { return _PreguntasPorUsuario; } set { _PreguntasPorUsuario = value; OnPropertyChanged("PreguntasPorUsuario"); } }
        private VMUsuarioAplicativo _Usuario;
        public VMUsuarioAplicativo Usuario { get { return _Usuario; } set { _Usuario = value; OnPropertyChanged("Usuario"); } }

        private CommandUsuarios _Comando;
        public CommandUsuarios Comando { get { if (_Comando == null) _Comando = new CommandUsuarios(this); return _Comando; } set { _Comando = value; } }
        public class CommandUsuarios : CommandObjeto
        {
            private VMPreguntasUsuario viewModelPadre;
            public CommandUsuarios(VMPreguntasUsuario _viewModelPadre)
            {
                viewModelPadre = _viewModelPadre;
            }
            public override void Execute(object parameter)
            {
                if (parameter is VMPreguntasUsuario)
                {
                    if (!viewModelPadre.EsActualizacion)
                    {
                        if (viewModelPadre.CuentaPreguntas == 3)
                        {
                            BackgroundWorker worker = new BackgroundWorker();
                            worker.DoWork += new DoWorkEventHandler(GuardaUsuarioDoWork);
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GuardaUsuarioCompleted);
                            //worker.RunWorkerAsync();
                        }
                        else
                        {
                            MessageBox.Show("No ha agregado todas las preguntas secretas requeridas");
                        }
                    }
                }
            }
            private void GuardaUsuarioDoWork(object sender, DoWorkEventArgs e)
            {
                try
                {
                    viewModelPadre.Usuario.HashAcceso = ComputarSha256(viewModelPadre.Usuario.PasswordClaro.Trim());
                    dtoUsuarioAplicativo UserNuevo = new dtoUsuarioAplicativo
                    {
                        Activo = true,
                        HashAcceso = viewModelPadre.Usuario.HashAcceso,
                        IdUsuario = viewModelPadre.Usuario.IdUsuario,
                        NombreUsuario = viewModelPadre.Usuario.NombreUsuario,
                    };
                    e.Result = new BALUsuarios().UpdateUsuarioAplicativo(UserNuevo);
                    Tuple<bool, string> _resultado = (Tuple<bool, string>)e.Result;
                    if (!_resultado.Item1)
                        throw new Exception(_resultado.Item2);
                }
                catch (Exception ex)
                {
                    throw new Exception(ConstantesComunes.ERROR_GENERICO);
                }
            }

            private void GuardaUsuarioCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    viewModelPadre.EsActualizacion = true;
                    Tuple<bool, string> _resultadoEjecucion = e.Result as Tuple<bool, string>;
                    viewModelPadre.Usuario.IdUsuario = Convert.ToInt32(_resultadoEjecucion.Item2);
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(GuardaUsuarioPreguntaDoWork);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GuardaPreguntasUsuarioCompleted);
                    worker.RunWorkerAsync();

                    MessageBox.Show(DTOWDXDespachoVilla.Constantes.ConstantesComunes.EXITO_GENERICO);
                }
                else
                {
                    MessageBox.Show(String.Format("Ha ocurrido el siguiente error: {0}", e.Error.Message.ToString()));
                }
            }

            private void GuardaUsuarioPreguntaDoWork(object sender, DoWorkEventArgs e)
            {
                try
                {
                    List<dtoPreguntasUsuarios> ListaPreguntas = new List<dtoPreguntasUsuarios>();
                    foreach (VMPreguntasAcceso Preguntas in viewModelPadre.PreguntasPorUsuario)
                    {
                        ListaPreguntas.Add(new dtoPreguntasUsuarios
                        {
                            Usuario = new dtoUsuarioAplicativo
                            {
                                Activo = true,
                                HashAcceso = ComputarSha256(viewModelPadre.Usuario.PasswordClaro),
                                IdUsuario = viewModelPadre.Usuario.IdUsuario,
                                NombreUsuario = viewModelPadre.Usuario.NombreUsuario
                            },
                            Pregunta = new dtoPreguntasAcceso
                            {
                                IdPregunta = Preguntas.IdPregunta,
                                Pregunta = Preguntas.Pregunta
                            }
                        });
                    }
                    e.Result = new BALUsuarios().UpdatePreguntasUsuarioAplicativo(ListaPreguntas);
                    Tuple<bool, string> _resultado = (Tuple<bool, string>)e.Result;
                    if (!_resultado.Item1)
                        throw new Exception(_resultado.Item2);
                }
                catch (Exception ex)
                {
                    throw new Exception(ConstantesComunes.ERROR_GENERICO);
                }
            }

            private void GuardaPreguntasUsuarioCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    MessageBox.Show(DTOWDXDespachoVilla.Constantes.ConstantesComunes.EXITO_GENERICO);
                }
                else
                {
                    MessageBox.Show(String.Format("Ha ocurrido el siguiente error: {0}", e.Error.Message.ToString()));
                }
            }

            public static string ComputarSha256(string RawData)
            {
                using (SHA256 Sha = SHA256.Create())
                {
                    byte[] arreglo = Sha.ComputeHash(Encoding.UTF8.GetBytes(RawData));
                    StringBuilder Sb = new StringBuilder();
                    for (int i = 0; i < arreglo.Length; i++)
                    {
                        Sb.Append(arreglo[i].ToString("X2"));
                    }
                    return Sb.ToString();
                }
            }
        }
    }
    public class VMPreguntasAcceso : VMBase
    {
        private bool _IsSelected;
        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        private int _IdPregunta;
        private string _Pregunta;
        public int IdPregunta { get { return _IdPregunta; } set { _IdPregunta = value; OnPropertyChanged("IdPregunta"); } }
        public string Pregunta
        {
            get { return _Pregunta; }
            set { _Pregunta = value; OnPropertyChanged("Pregunta"); }
        }
        private string _Respuesta;
        public string Respuesta { get { return _Respuesta; } set { _Respuesta = value; OnPropertyChanged("Respuesta"); } }
    }
    public class VMUsuarioAplicativo : VMBase
    {
        private int _IdUsuario;
        private string _NombreUsuario;
        private string _PasswordClaro;
        public string PasswordClaro { get { return _PasswordClaro; } set { _PasswordClaro = value; OnPropertyChanged("PasswordClaro"); } }
        public int IdUsuario { get { return _IdUsuario; } set { _IdUsuario = value; OnPropertyChanged("IdUsuario"); } }
        public string NombreUsuario { get { return _NombreUsuario; } set { _NombreUsuario = value; OnPropertyChanged("NombreUsuario"); } }
        public string HashAcceso { get; set; }
        public bool Activo { get; set; }
    }
}
