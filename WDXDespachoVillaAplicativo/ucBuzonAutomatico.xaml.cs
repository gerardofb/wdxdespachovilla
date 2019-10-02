using Google.Apis.Gmail.v1.Data;
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
using WDXDespachoVillaAplicativo.Servicios;
using WDXDespachoVillaAplicativo.ViewModel.Buzon;

namespace WDXDespachoVillaAplicativo
{
    /// <summary>
    /// Interaction logic for BuzonAutomatico.xaml
    /// </summary>
    public partial class ucBuzonAutomatico : UserControl
    {
        public VMBuzon viewModel { get; set; }
        public ucBuzonAutomatico()
        {
            InitializeComponent();
            SetViewModel();
        }
        private void SetViewModel()
        {
            viewModel = new VMBuzon();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ConsultaMensajesDoWork;
            worker.RunWorkerCompleted += ConsultaMensajesCompleted;
            worker.RunWorkerAsync();
        }

        private void ConsultaMensajesDoWork(object sender, DoWorkEventArgs e)
        {
            ServicioGmail Servicio = new ServicioGmail();
            string asuntos = "subject:Solicitud automática de nuevo asunto con el despacho is:unread";
            e.Result = Servicio.GetBandejaPorAsunto(asuntos);
        }

        private void ConsultaMensajesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                viewModel.ListaMensajes = new List<VMBuzon>();
                if (e.Result != null)
                {
                    foreach (Message mensaje in e.Result as IList<Message>)
                    {
                        string Cuerpo = mensaje.Payload.Parts.FirstOrDefault(x => x.MimeType == "text/plain").Body.Data;
                        string CuerpoDecodificado = Encoding.UTF8.GetString(ServicioGmail.FromBase64ForUrlString(Cuerpo));
                        string Codigo = CuerpoDecodificado.IndexOf("[código]") > 0 ? CuerpoDecodificado.Substring(CuerpoDecodificado.IndexOf("[código]") + 8, 64) : String.Empty;
                        DateTime epoca = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                        DateTime fecha = epoca.AddMilliseconds((double)mensaje.InternalDate);
                        DateTime fechaIntermedia = DateTimeOffset.FromUnixTimeMilliseconds((long)mensaje.InternalDate).UtcDateTime;
                        string Subject = mensaje.Payload.Headers.FirstOrDefault(k => k.Name == "Subject").Value;
                        viewModel.ListaMensajes.Add(new VMBuzon
                        {
                            Asunto = Subject,
                            FechaHora = fecha.ToString("dd/MM/yyyy HH:mm"),
                            Mensaje = CuerpoDecodificado,
                            Codigo = Codigo
                        });
                    }
                }
                this.DataContext = viewModel;
            }
        }

        private void CheckAsuntoAutomatico_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            VMBuzon elemento = check.DataContext as VMBuzon;
            elemento.Selected = true;
        }
        private void CheckAsuntoAutomatico_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            VMBuzon elemento = check.DataContext as VMBuzon;
            elemento.Selected = false;
        }
    }
}
