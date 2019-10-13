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
using WDXDespachoVillaAplicativo.ViewModel.Personas;

namespace WDXDespachoVillaAplicativo
{
    public class ValidacionPersona : System.Windows.Controls.ValidationRule
    {
        public override System.Windows.Controls.ValidationResult Validate(object value, System.Globalization.CultureInfo Cultura)
       {
            string valor = (string)value;
            valor = valor.Trim();
            if (String.IsNullOrEmpty(valor))
            {
                return new System.Windows.Controls.ValidationResult(false, "Este campo es obligatorio");
            }
            return System.Windows.Controls.ValidationResult.ValidResult;
        }
    }
    /// <summary>
    /// Interaction logic for AgregaPersona.xaml
    /// </summary>
    public partial class AgregaPersona : Window
    {
        private Delegate DelegadoAgregaPersona;
        public VMPersonas viewModel { get; set; }
        public AgregaPersona(Delegate AgregaPersona)
        {
            InitializeComponent();
            DelegadoAgregaPersona = AgregaPersona;
            SetViewModel();
        }
        private void SetViewModel()
        {
            this.viewModel = new VMPersonas(DelegadoAgregaPersona, this);
            viewModel.ListadoPersonas = new List<VMPersonas>();
            viewModel.ListadoPersonas.Add(this.viewModel);
            this.DataContext = viewModel;
        }
    }
}
