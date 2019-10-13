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
using WDXDespachoVillaAplicativo.ViewModel.Secciones;
using System.ComponentModel;
using BALWDXDespachoVilla.Secciones;
using DTOWDXDespachoVilla.Secciones;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for ucAdminSecciones.xaml
    /// </summary>
    public partial class ucAdminSecciones : UserControl
    {
        private VMSecciones viewModel;
        public ucAdminSecciones()
        {
            InitializeComponent();
            SetViewModel();
        }
        private void SetViewModel()
        {
            viewModel = new VMSecciones(this);
            this.DataContext = viewModel;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ConsultaSeccionesDoWork;
            worker.RunWorkerCompleted += ConsultaSeccionesDoWorkCompleted;
            worker.RunWorkerAsync();
        }
        private void ConsultaSeccionesDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = new BALSecciones().GetCatalogoSecciones(1);
        }
        private void ConsultaSeccionesDoWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>> _resultado =
                e.Result as Tuple<string, List<dtoCatSecciones>, List<dtoSeccion>>;
            if(_resultado != null && _resultado.Item1 == "")
            {
                foreach(dtoCatSecciones elem in _resultado.Item2)
                {
                    viewModel.ListadoSecciones.Add(new VMSecciones
                    {
                        IdSeccion = elem.IdSeccion,
                        Seccion = elem.Seccion
                    });
                }
            }
            this.ComboSecciones.ItemsSource = viewModel.ListadoSecciones;
            this.ComboSecciones.Items.Refresh();
        }

        private void SeccionTexto_TextChanged(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
