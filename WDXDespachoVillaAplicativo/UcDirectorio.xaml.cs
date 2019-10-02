using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WDXDespachoVillaAplicativo.ViewModel.Personas;
using DTOWDXDespachoVilla.Personas;
using System.ComponentModel;
using WDXDespachoVillaAplicativo.ViewModel.Mensajes;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for UcDirectorio.xaml
    /// </summary>
    public partial class UcDirectorio : UserControl
    {
        public delegate void DelAgregaPersona();
        public DelAgregaPersona DelegadoPersona { get; set; }
        public VMPersonas viewModel { get; set; }
        public UcDirectorio()
        {
            InitializeComponent();
            SetViewModel();
        }

        private void SetViewModel()
        {
            DelegadoPersona = new DelAgregaPersona(AgregaPersonaDirectorio);
            viewModel = new VMPersonas();
            this.GridPersonas.ItemsSource = viewModel.ListadoPersonas;
            AgregaPersonaDirectorio();

        }
        private void AgregaPersonaDirectorio()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(ConsultaPersonasDoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConsultaPersonasCompleted);
            worker.RunWorkerAsync();
        }

        private void ConsultaPersonasDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = new BALWDXDespachoVilla.Personas.BALPersonas().GetPersonasDirectorio();
        }

        private void ConsultaPersonasCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<dtoPersonas> _resultado = e.Result as List<dtoPersonas>;
            if (_resultado != null)
            {
                //viewModel.ListadoPersonas.Clear();
                List<VMPersonas> ListadoNuevo = new List<VMPersonas>();
                foreach (dtoPersonas Persona in _resultado)
                {
                    ListadoNuevo.Add(new VMPersonas
                    {
                        Nombres = Persona.Nombres,
                        ApellidoMaterno = Persona.ApellidoMaterno,
                        ApellidoPaterno = Persona.ApellidoPaterno,
                        Calle = Persona.Calle,
                        CodigoPostal = Persona.CodigoPostal,
                        Colonia = Persona.Colonia,
                        EntidadFederativa = Persona.EntidadFederativa,
                        Localidad = Persona.Localidad,
                        NumeroExterior = Persona.NumeroExterior,
                        NumeroInterior = Persona.NumeroInterior,
                        Telefono = Persona.Telefono
                    });
                }
                viewModel.ListadoPersonas = ListadoNuevo;
                GridPersonas.ItemsSource = viewModel.ListadoPersonas;
            }
        }

        private void btnAgregaDirectorio_Click(object sender, RoutedEventArgs e)
        {
            AgregaPersona ventana = new AgregaPersona(DelegadoPersona);
            ventana.ShowDialog();
        }

        private void CheckPersonaMensaje(object sender, RoutedEventArgs e)
        {
            CheckBox elem = e.Source as CheckBox;
            VMPersonas variable = elem.DataContext as VMPersonas;
            if (variable != null)
            {
                this.viewModel.ListadoPersonas.FirstOrDefault(k => k.Telefono == variable.Telefono).IsSelected = true;
                this.viewModel.ListadoPersonas.Where(k => k.Telefono != variable.Telefono).ToList().ForEach(x => x.IsSelected = false);
            }
        }

        private void UncheckPersonaMensaje(object sender, RoutedEventArgs e)
        {
            CheckBox elem = e.Source as CheckBox;
            VMPersonas variable = elem.DataContext as VMPersonas;
            if (variable != null)
            {
                this.viewModel.ListadoPersonas.FirstOrDefault(k => k.Telefono == variable.Telefono).IsSelected = false;
            }
        }

        private void EnviarMensaje_Click(object sender, RoutedEventArgs e)
        {
            MensajesTexto ventana = new MensajesTexto();
            ventana.viewModel = new ViewModel.Mensajes.VMMensajes(ventana);
            List<VMPersonas> ListadoPersonas = this.GridPersonas.Items.SourceCollection as List<VMPersonas>;
            if (ListadoPersonas != null)
            {
                VMMensajes VMventana = new VMMensajes(ventana);
                VMventana.ListadoPersonas = ListadoPersonas.Where(x => x.IsSelected).ToList();
                VMventana.CuentaMensajes = ListadoPersonas.Count;
                ventana.SetViewModel(VMventana);
            }
            ventana.ShowDialog();
        }

    }
}
