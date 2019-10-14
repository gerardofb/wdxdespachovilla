using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BALWDXDespachoVilla.Secciones;
using DTOWDXDespachoVilla.Secciones;
using System.Windows.Controls;

namespace WDXDespachoVillaAplicativo.ViewModel.Secciones
{
    public class VMSecciones : VMBase
    {
        public VMSecciones()
        {

        }
        public VMSecciones(ucAdminSecciones ventana)
        {
            ventanaPadre = ventana;
            ListadoSecciones = new List<VMSecciones>();
        }
        public ucAdminSecciones ventanaPadre { get; set; }
        private int _IdSeccion;
        public int IdSeccion
        {
            get { return _IdSeccion; }
            set { _IdSeccion = value; OnPropertyChanged("IdSeccion"); }
        }
        private VMSecciones _SeccionElegida;
        public VMSecciones SeccionElegida { get { return _SeccionElegida; } set { _SeccionElegida = value; OnPropertyChanged("SeccionElegida"); } }
        private string _Seccion;
        public string Seccion { get { return _Seccion; } set { _Seccion = value; OnPropertyChanged("Seccion"); } }
        private string _TextoSeccion;
        public string TextoSeccion { get { return _TextoSeccion; } set { _TextoSeccion = value; OnPropertyChanged("TextoSeccion"); } }
        private List<VMSecciones> _ListadoSecciones;
        public List<VMSecciones> ListadoSecciones { get { return _ListadoSecciones; } set { _ListadoSecciones = value; OnPropertyChanged("ListadoSecciones"); } }

        private CommandSecciones _Comando;
        public CommandSecciones Comando { get { if (_Comando == null) _Comando = new CommandSecciones(this); return _Comando; } set { _Comando = value; } }

        public class CommandSecciones : CommandObjeto
        {
            public CommandSecciones(VMSecciones instancia)
            {
                viewModelPadre = instancia;
            }
            private VMSecciones viewModelPadre;
            
            public override void Execute(object parameter)
            {
                if(parameter is VMSecciones)
                {
                    VMSecciones Seccion = this.viewModelPadre.ventanaPadre.ComboSecciones.SelectedItem as VMSecciones;
                    string TextoSeccion = viewModelPadre.TextoSeccion;
                    string[] TextoSeparado = TextoSeccion.Split('\n');
                    TextoSeccion = String.Empty;
                    string TextoParrafo = @"<p class='SeccionesParrafo'>";
                    for(int i = 0; i < TextoSeparado.Length; i++)
                    {
                        TextoSeccion+= (i== 0?TextoParrafo:"<p>") + TextoSeparado[i] + "</p>";
                    }

                    this.viewModelPadre.SeccionElegida = new VMSecciones
                    {
                        IdSeccion = Seccion.IdSeccion,
                        TextoSeccion = TextoSeccion
                    };
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(this.GuardaSeccionDoWork);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.GuardaSeccionCompleted);
                    worker.RunWorkerAsync();
                }
            }
            private void GuardaSeccionDoWork(object sender, DoWorkEventArgs e)
            {
                dtoSeccion SeccionNueva = new dtoSeccion
                {
                    IdSeccion = this.viewModelPadre.SeccionElegida.IdSeccion,
                    Texto = this.viewModelPadre.SeccionElegida.TextoSeccion
                };
                e.Result = new BALSecciones().UpdateSeccion(SeccionNueva);
                Tuple<bool, string> _resultado = e.Result as Tuple<bool, string>;
                if(_resultado.Item1 == false)
                {
                    throw new Exception(_resultado.Item2);
                }
            }
            private void GuardaSeccionCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if(e.Error == null)
                {
                    Tuple<bool, string> _resultado = e.Result as Tuple<bool, string>;
                    MessageBox.Show(_resultado.Item2);
                }
                else
                {
                    MessageBox.Show(e.Error.Message.ToString());
                }
            }
        }
    }
}
