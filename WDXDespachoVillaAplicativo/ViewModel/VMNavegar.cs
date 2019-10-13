using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDXDespachoVillaAplicativo.ViewModel.Buzon;

namespace WDXDespachoVillaAplicativo.ViewModel
{
    public class VMNavegar : VMBase
    {
        public PersonasDirectorio ventanaPadre { get; set; }
        public VMNavegar(PersonasDirectorio ventana)
        {
            this.ventanaPadre = ventana;
        }
        private CommandNavegacion _CmdNavegar;
        public CommandNavegacion CmdNavegar { get { if (_CmdNavegar == null) return new CommandNavegacion(this); else return _CmdNavegar; } set { _CmdNavegar = value; } }
        public class CommandNavegacion : CommandObjeto
        {
            private VMNavegar instanciaViewModel;
            public CommandNavegacion(VMNavegar instancia)
            {
                instanciaViewModel = instancia;
            }
            public override void Execute(object parameter)
            {
                int Opcion = Convert.ToInt32(parameter);
                switch (Opcion)
                {
                    case 0:
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Clear();
                        UcDirectorio ventana = new UcDirectorio();
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Add(ventana);
                        break;
                    case 1:
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Clear();
                        ucBuzonAutomatico ventanaBuzon = new ucBuzonAutomatico();
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Add(ventanaBuzon);
                        break;
                    case 2:
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Clear();
                        ucDescargaArchivosAsuntos ventanaArchivos = new ucDescargaArchivosAsuntos();
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Add(ventanaArchivos);
                        break;
                    case 3:
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Clear();
                        ucAdminSecciones ventanaSecciones = new ucAdminSecciones();
                        instanciaViewModel.ventanaPadre.Contenedor.Children.Add(ventanaSecciones);
                        break;
                }
            }
        }
    }
}
