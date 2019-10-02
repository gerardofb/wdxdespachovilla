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
using System.Windows.Shapes;
using WDXDespachoVillaAplicativo.ViewModel.Usuarios;
using BALWDXDespachoVilla.Usuarios;
using System.ComponentModel;
using DTOWDXDespachoVilla.Constantes;
using DTOWDXDespachoVilla.Usuarios;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for Registro.xaml
    /// </summary>
    public partial class Registro : Window
    {
        VMPreguntasUsuario viewModel;
        public Registro()
        {
            InitializeComponent();
            SetViewModel();
        }

        private void SetViewModel()
        {
            viewModel = new VMPreguntasUsuario();
            this.DataContext = viewModel;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ConsultaPreguntasDoWork;
            worker.RunWorkerCompleted += ConsultaPreguntasCompleted;
            worker.RunWorkerAsync();
        }
        private void ConsultaPreguntasDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = new BALUsuarios().GetPreguntasAccesoAplicativo();
                Tuple<bool,List<dtoPreguntasAcceso>> control = e.Result as Tuple<bool, List<dtoPreguntasAcceso>>;
                if (control != null && !control.Item1)
                    throw new Exception(ConstantesComunes.ERROR_GENERICO);
            }
            catch (Exception ex)
            {
                throw new Exception(ConstantesComunes.ERROR_GENERICO);
            }
        }

        private void ConsultaPreguntasCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Tuple<bool, List<dtoPreguntasAcceso>> Listado = e.Result as Tuple<bool, List<dtoPreguntasAcceso>>;
                viewModel.PreguntasPorUsuario = new List<VMPreguntasAcceso>();
                foreach(dtoPreguntasAcceso elem in Listado.Item2)
                {
                    viewModel.PreguntasPorUsuario.Add(new VMPreguntasAcceso
                    {
                         IdPregunta = elem.IdPregunta,
                         Pregunta = elem.Pregunta
                    });
                }
                this.ComboPreguntasSecretas.ItemsSource = viewModel.PreguntasPorUsuario;
                this.ComboPreguntasSecretas.Items.Refresh();
            }
            else
            {
                MessageBox.Show(DTOWDXDespachoVilla.Constantes.ConstantesComunes.ERROR_GENERICO);
            }
        }
        private void buttonElijeRespuesta_Click(object sender, RoutedEventArgs e)
        {
            VMPreguntasAcceso Pregunta = this.ComboPreguntasSecretas.SelectedItem as VMPreguntasAcceso;
            if(Pregunta != null && viewModel.RespuestaElegida.Trim() != "")
            {
                this.viewModel.PreguntasPorUsuario.FirstOrDefault(x => x.IdPregunta == Pregunta.IdPregunta).Respuesta = viewModel.RespuestaElegida;
                this.viewModel.PreguntasPorUsuario.FirstOrDefault(x => x.IdPregunta == Pregunta.IdPregunta).IsSelected = true;

                viewModel.CuentaPreguntas++;
                if(viewModel.CuentaPreguntas >=3)
                {
                    this.ResponderYContar.IsEnabled = false;
                    this.CuentaRespuestas.Content = @"Ha respondido todas las preguntas requeridas, puede registrarse ahora";
                }
            }
        }
    }
}
