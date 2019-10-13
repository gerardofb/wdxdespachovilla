using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WDXDespachoVillaAplicativo.ViewModel.Mensajes
{
    public class VMMensajes : VMBase
    {
        public VMMensajes(MensajesTexto _ventana)
        {
            this.ventana = _ventana;
        }
        private MensajesTexto ventana;
        private List<ViewModel.Personas.VMPersonas> _ListadoPersonas;
        public List<ViewModel.Personas.VMPersonas> ListadoPersonas { get { return _ListadoPersonas; } set { _ListadoPersonas = value; OnPropertyChanged("ListadoPersonas"); } }
        private string _Texto;
        public string Texto { get { return _Texto; } set { _Texto = value; OnPropertyChanged("Texto"); } }
        private int _CuentaMensajes;
        public int CuentaMensajes { get { return _CuentaMensajes; } set { _CuentaMensajes = value != ListadoPersonas.Count ? ListadoPersonas.Count : value; OnPropertyChanged("CuentaMensajes"); } }
        private string _LabelListado;
        public string LabelListado { get { return _LabelListado; } set { _LabelListado = value; OnPropertyChanged("LabelListado"); } }
        private CommandMensajes _Comando;
        public CommandMensajes Comando
        {
            get { if (_Comando == null) _Comando = new CommandMensajes(this); return _Comando; }
            set { _Comando = value; }
        }
        public class CommandMensajes : CommandObjeto
        {
            public CommandMensajes(VMMensajes instancia)
            {
                viewModelPadre = instancia;
            }
            private VMMensajes viewModelPadre;
            public override void Execute(object parameter)
            {
                List<ViewModel.Personas.VMPersonas> PersonasMensaje = viewModelPadre.ListadoPersonas;
                if (!String.IsNullOrEmpty(viewModelPadre.Texto) && PersonasMensaje.Count > 0)
                {
                    ViewModel.Personas.VMPersonas Persona = PersonasMensaje.First();
                    string Texto = viewModelPadre.Texto.Trim();
                    if(viewModelPadre.ventana != null)
                    {
                        string Telefono = Persona.Telefono;
                        Regex patron = new Regex(@"[\(\)\+\-\s]");
                        Telefono = patron.Replace(Telefono, "");
                        string TextoEnc = System.Web.HttpUtility.UrlEncode(Texto);
                        Regex patronLada = new Regex(@"^521\d+$");
                        if (patronLada.Match(Telefono).Success == false)
                        {
                            Telefono = "521" + Telefono;
                        }
                        string Url = $"https://wa.me/{Telefono}?text={TextoEnc}";
                        Process.Start("Chrome.exe", Url);
                    }
                }
            }
        }
    }
}
