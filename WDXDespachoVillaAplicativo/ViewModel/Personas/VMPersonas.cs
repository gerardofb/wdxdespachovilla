using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOWDXDespachoVilla.Personas;
using BALWDXDespachoVilla.Personas;
using System.ComponentModel;

namespace WDXDespachoVillaAplicativo.ViewModel.Personas
{
    public class VMPersonas : VMBase
    {
        public VMPersonas()
        {
            ListadoPersonas = new List<VMPersonas>();
        }
        public VMPersonas(Delegate AgregaPersona, AgregaPersona ventanaPadre)
        {
            Ventana = ventanaPadre;
            DelegadoAgregaPersona = AgregaPersona;
            ListadoPersonas = new List<VMPersonas>();
        }
        private AgregaPersona Ventana;
        private Delegate DelegadoAgregaPersona;
        private string _Nombres;
        private string _Apellidos;
        private string _ApellidoPaterno;
        private string _ApellidoMaterno;
        private string _Direccion;
        private string _Calle;
        private string _NumeroExterior;
        private string _NumeroInterior;
        private string _CodigoPostal;
        private string _EntidadFederativa;
        private string _Colonia;
        private string _Localidad;
        private string _Telefono;
        private bool _IsSelected;
        private List<VMPersonas> _ListadoPersonas;

        public bool IsSelected { get { return _IsSelected; }set { _IsSelected = value;  OnPropertyChanged("IsSelected"); } }
        public string Nombres { get { return _Nombres; } set { _Nombres = value; OnPropertyChanged("Nombres"); } }
        public string Apellidos { get { return String.Format("{0} {1}", this.ApellidoPaterno, this.ApellidoMaterno); } }
        public string ApellidoPaterno { get { return _ApellidoPaterno; } set { _ApellidoPaterno = value; OnPropertyChanged("ApellidoPaterno"); } }
        public string ApellidoMaterno { get { return _ApellidoMaterno; } set { _ApellidoMaterno = value; OnPropertyChanged("ApellidoMaterno"); } }
        public string Calle { get { return _Calle; } set { _Calle = value; OnPropertyChanged("Calle"); } }
        public string NumeroExterior { get { return _NumeroExterior; } set { _NumeroExterior = value; OnPropertyChanged("NumeroExterior"); } }
        public string NumeroInterior { get { return _NumeroInterior; } set { _NumeroInterior = value; OnPropertyChanged("NumeroInterior"); } }
        public string EntidadFederativa { get { return _EntidadFederativa; }set { _EntidadFederativa = value; OnPropertyChanged("EntidadFederativa"); } }
        public string CodigoPostal { get { return _CodigoPostal; } set { _CodigoPostal = value; OnPropertyChanged("CodigoPostal"); } }
        public string Direccion { get { return string.Format("{0} {1} {2}", this.Calle, this.NumeroExterior, this.NumeroInterior); } }
        public string Colonia { get { return _Colonia; } set { _Colonia = value; OnPropertyChanged("Colonia"); } }
        public string Localidad { get { return _Localidad; } set { _Localidad = value; OnPropertyChanged("Localidad"); } }
        public string Telefono { get { return _Telefono; } set { _Telefono = value; OnPropertyChanged("Telefono"); } }

        public List<VMPersonas> ListadoPersonas { get { return _ListadoPersonas; } set { _ListadoPersonas = value; OnPropertyChanged("ListadoPersonas"); } }

        private CommandPersonas _Comando;

        public CommandPersonas Comando
        {
            get { if (_Comando == null) _Comando = new CommandPersonas(this); return _Comando; }
            set { _Comando = value; }
        }
        public class CommandPersonas : CommandObjeto
        {
            public CommandPersonas(VMPersonas instancia)
            {
                viewModelPadre = instancia;
            }
            private VMPersonas viewModelPadre;
            protected List<dtoPersonas> ListadoGuardar { get; set; }

            public override void Execute(object parameter)
            {
                if (parameter is VMPersonas)
                {
                    VMPersonas Elemento = parameter as VMPersonas;
                    ListadoGuardar = new List<dtoPersonas>();
                    ListadoGuardar.Add(new dtoPersonas
                    {
                        ApellidoPaterno = Elemento.ApellidoPaterno,
                        ApellidoMaterno = Elemento.ApellidoMaterno,
                        Nombres = Elemento.Nombres,
                        Calle = Elemento.Calle,
                        CodigoPostal = Elemento.CodigoPostal,
                        Colonia = Elemento.Colonia,
                        EntidadFederativa = Elemento.EntidadFederativa,
                        IdPersona = 0,
                        Localidad = Elemento.Localidad,
                        NumeroExterior = Elemento.NumeroExterior,
                        NumeroInterior = Elemento.NumeroInterior,
                        Telefono = Elemento.Telefono
                    });
                                        
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(this.GuardaPersonaDoWork);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.GuardaPersonaCompleted);
                    worker.RunWorkerAsync();
                }
            }

            private void GuardaPersonaDoWork(object sender, DoWorkEventArgs e)
            {
                e.Result = new BALPersonas().UpdatePersona(ListadoGuardar);
            }

            private void GuardaPersonaCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if(e.Result as Tuple<bool, string> != null)
                {
                    Tuple<bool, string> Resultado = (Tuple<bool, string>)e.Result;
                    if (Resultado.Item1 == true)
                    {
                        viewModelPadre.Ventana.Close();
                        this.viewModelPadre.DelegadoAgregaPersona.DynamicInvoke();
                    }
                }
            }
        }
    }
}
