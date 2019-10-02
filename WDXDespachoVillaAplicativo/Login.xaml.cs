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
using WDXDespachoVillaAplicativo.ViewModel.Usuarios;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public VMUsuarios viewModel { get; set; }
        public Login()
        {
            InitializeComponent();
            SetViewModel();
        }

        public void SetViewModel()
        {
            this.viewModel = new VMUsuarios();
            this.DataContext = viewModel;
        }

        private void btnRegistroClick(object sender, RoutedEventArgs e)
        {
            Registro ventana = new Registro();
            ventana.ShowDialog();
        }
    }
}
