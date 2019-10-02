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
using WDXDespachoVillaAplicativo.ViewModel.Mensajes;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for MensajesTexto.xaml
    /// </summary>
    public partial class MensajesTexto : Window
    {
        public VMMensajes viewModel { get; set; }
        public MensajesTexto()
        {
            InitializeComponent();
        }
        public void SetViewModel(VMMensajes mensaje)
        {
            this.viewModel = mensaje;
            viewModel.LabelListado = String.Format("Ha seleccionado enviar un mensaje a {0} personas", viewModel.CuentaMensajes);
            GridMensajesTexto.DataContext = this.viewModel;
        }
    }
}
