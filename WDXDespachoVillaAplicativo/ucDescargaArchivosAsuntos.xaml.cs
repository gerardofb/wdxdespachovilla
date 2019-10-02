using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WDXDespachoVillaAplicativo.ViewModel.Asuntos;
using BALWDXDespachoVilla.Asuntos;
using DTOWDXDespachoVilla.Asuntos;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for ucDescargaArchivosAsuntos.xaml
    /// </summary>
    public partial class ucDescargaArchivosAsuntos : UserControl
    {
        public VMAsuntos viewModel { get; set; }
        public ucDescargaArchivosAsuntos()
        {
            InitializeComponent();
            SetViewModel();
        }
        private void SetViewModel()
        {
            this.viewModel = new VMAsuntos();
            this.DataContext = viewModel;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(ArchivosAsuntoDoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AsuntosArchivosCompleted);
            worker.RunWorkerAsync();
        }

        private void CheckArchivoAsunto(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            VMAsuntos elemento = check.DataContext as VMAsuntos;
            if(elemento != null)
            {
                elemento.IsSelected = true;
            }
        }

        private void UnCheckArchivoAsunto(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            VMAsuntos elemento = check.DataContext as VMAsuntos;
            if (elemento != null)
            {
                elemento.IsSelected = false;
            }
        }

        private void ArchivosAsuntoDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = new BALAsuntos().GetAllAsuntosArchivos();
        }

        private void AsuntosArchivosCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<dtoAsuntos> _resultado = e.Result as List<dtoAsuntos>;
                if (_resultado != null)
                {
                    viewModel.ListadoAsuntos = new List<VMAsuntos>();
                    foreach (dtoAsuntos archivo in _resultado)
                    {
                        viewModel.ListadoAsuntos.Add(new VMAsuntos
                        {
                            FechaAprobacion = archivo.FechaAprobacion,
                            FechaArchivo = archivo.FechaArchivo,
                            FechaCreacion = archivo.FechaCreacion,
                            Email = archivo.Email,
                            Archivo = archivo.Archivo,
                            Asunto = archivo.Asunto,
                            Drive = archivo.Drive,
                            GuidArchivo = archivo.GuidArchivo,
                            IdAsunto = archivo.IdAsunto,
                            IdUsuario = archivo.IdUsuario,
                            NombreArchivo = archivo.NombreArchivo,
                            DriveCompartido = archivo.DriveCompartido,
                        });
                    }
                    viewModel.ListadoFinal = CollectionViewSource.GetDefaultView(viewModel.ListadoAsuntos);
                    if (viewModel.ListadoFinal != null && viewModel.ListadoFinal.CanGroup == true)
                    {
                        viewModel.ListadoFinal.GroupDescriptions.Clear();
                        viewModel.ListadoFinal.GroupDescriptions.Add(new PropertyGroupDescription("Email"));
                        viewModel.ListadoFinal.GroupDescriptions.Add(new PropertyGroupDescription("Asunto"));
                    }
                    this.GridArchivosAsuntos.ItemsSource = viewModel.ListadoFinal;

                }
            }
        }
    }
}
