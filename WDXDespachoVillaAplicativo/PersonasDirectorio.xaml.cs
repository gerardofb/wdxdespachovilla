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
using WDXDespachoVillaAplicativo.ViewModel;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for PersonasDirectorio.xaml
    /// </summary>
    public partial class PersonasDirectorio : Window
    {
        private VMNavegar viewModel;
        public PersonasDirectorio()
        {
            InitializeComponent();
            SetViewModel();
        }
        private void SetViewModel()
        {
            this.viewModel = new VMNavegar(this);
            this.DataContext = viewModel;
        }
    }
}
