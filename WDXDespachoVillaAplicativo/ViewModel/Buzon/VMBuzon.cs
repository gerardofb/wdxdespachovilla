using BALWDXDespachoVilla.Asuntos;
using DTOWDXDespachoVilla.Asuntos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace WDXDespachoVillaAplicativo.ViewModel.Buzon
{
    public class VMBuzon : VMBase
    {
        private string _Asunto;
        private string _Codigo;
        private string _Mensaje;
        private string _FechaHora;
        public string Usuario { get; set; }
        public string Asunto { get { return _Asunto; } set { _Asunto = value; OnPropertyChanged("Asunto"); } }
        public string Codigo { get { return String.Format("Código: {0}", _Codigo); } set { _Codigo = value; OnPropertyChanged("Codigo"); } }
        public string Mensaje { get { return _Mensaje; } set { _Mensaje = value; OnPropertyChanged("Mensaje"); } }
        public string FechaHora { get { return _FechaHora; } set { _FechaHora = value; OnPropertyChanged("FechaHora"); } }
        private string _Encabezado;
        public string Encabezado { get { return String.Format("Fecha: {0} {1}", _FechaHora, Asunto); } set { _Encabezado = value; OnPropertyChanged("Encabezado"); } }
        private List<VMBuzon> _ListaMensajes;
        public List<VMBuzon> ListaMensajes { get { return _ListaMensajes; } set { _ListaMensajes = value; OnPropertyChanged("ListaMensajes"); } }
        private bool _Selected;
        public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("Selected"); } }

        private CommandAsuntosAutomaticos _Comando;
        public CommandAsuntosAutomaticos Comando { get { if (_Comando == null) _Comando = new CommandAsuntosAutomaticos(this); return _Comando; } set { _Comando = value; } }

        public class CommandAsuntosAutomaticos : CommandObjeto
        {
            public CommandAsuntosAutomaticos(VMBuzon instancia)
            {
                viewModelPadre = instancia;
            }
            private VMBuzon viewModelPadre;
            public override void Execute(object parameter)
            {
                List<VMBuzon> Listado = parameter as List<VMBuzon>;
                if (parameter != null)
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(GuardaAsuntoDoWork);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GuardaAsuntoCompleted);
                    worker.RunWorkerAsync();
                }
            }
            private void GuardaAsuntoDoWork(object sender, DoWorkEventArgs e)
            {
                foreach (VMBuzon elemento in viewModelPadre.ListaMensajes.Where(x => x.Selected))
                {
                    string Asunto = elemento.Mensaje.Substring(elemento.Mensaje.IndexOf("jurídico:") + 10, (elemento.Mensaje.IndexOf(".") - (elemento.Mensaje.IndexOf("jurídico:") + 10)));
                    Regex Expresion = new Regex(@"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*");
                    string Usuario = Expresion.Match(elemento.Mensaje).Value;
                    Usuario = Usuario.Substring(0, Usuario.Length);
                    dtoAsuntos asunto = new dtoAsuntos
                    {
                        Asunto = Asunto,
                        HashAsunto = elemento.Codigo,
                        Email = Usuario
                    };
                    if (ComputarSha256(String.Format("{0}_{1}", asunto.Email, asunto.Asunto)) != elemento.Codigo.Replace("Código:","").Trim())
                    {
                        throw new Exception(DTOWDXDespachoVilla.Constantes.ConstantesComunes.VALIDACION_CODIGO_HASH_FALLIDA);
                    }
                    e.Result = new BALAsuntos().UpdateAsuntos(asunto, true);
                    Tuple<bool, string> _resultado = (Tuple<bool, string>)e.Result;
                    if (!_resultado.Item1)
                        throw new Exception(_resultado.Item2);
                }
            }

            private void GuardaAsuntoCompleted(object sender, RunWorkerCompletedEventArgs e)
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
}
