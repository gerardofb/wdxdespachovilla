using BALWDXDespachoVilla.Asuntos;
using DTOWDXDespachoVilla.Asuntos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace WDXDespachoVillaAplicativo.ViewModel.Asuntos
{
    public class VMAsuntos : VMBase
    {
        public ICollectionView ListadoFinal { get; set; }
        private List<VMAsuntos> _ListadoAsuntos;
        public List<VMAsuntos> ListadoAsuntos { get { return _ListadoAsuntos; } set { _ListadoAsuntos = value; OnPropertyChanged("ListadoAsuntos"); } }
        private bool _IsSelected;
        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        public string IdUsuario { get; set; }
        public string Email { get; set; }
        public string Asunto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string HashAsunto { get; set; }
        public byte[] Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string GuidArchivo { get; set; }
        public int IdAsunto { get; set; }
        public string Tema { get; set; }
        public DateTime? FechaArchivo { get; set; }
        public bool Drive { get; set; }
        public bool DriveCompartido { get; set; }

        private CmdAsuntos _Comando;
        public CmdAsuntos Comando { get { if (_Comando == null) _Comando = new CmdAsuntos(this); return _Comando; } set { _Comando = value; OnPropertyChanged("Comando"); } }

        public class CmdAsuntos : CommandObjeto
        {
            public CmdAsuntos(VMAsuntos instancia)
            {
                viewModelPadre = instancia;
            }
            private VMAsuntos viewModelPadre;
            public override void Execute(object parameter)
            {
                if (parameter is VMAsuntos)
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += DescargaArchivoAsuntoDoWork;
                    worker.RunWorkerCompleted += DescargaAsuntoArchivoCompleted;
                    worker.RunWorkerAsync(); 
                }
            }

            private void DescargaArchivoAsuntoDoWork(object sender, DoWorkEventArgs e)
            {
                foreach (VMAsuntos elemento in viewModelPadre.ListadoAsuntos.Where(x => x.IsSelected))
                {
                    dtoAsuntos Asunto = new dtoAsuntos
                    {
                        Asunto = elemento.Asunto,
                        Drive = elemento.Drive,
                        DriveCompartido = elemento.DriveCompartido,
                        FechaAprobacion = elemento.FechaAprobacion,
                        FechaCreacion = elemento.FechaCreacion,
                        FechaArchivo = elemento.FechaArchivo,
                        Email = elemento.Email,
                        Archivo = elemento.Archivo,
                        IdAsunto = elemento.IdAsunto,
                        IdUsuario = elemento.IdUsuario,
                        GuidArchivo = elemento.GuidArchivo,
                        NombreArchivo = elemento.NombreArchivo
                    };
                    e.Result = new BALAsuntos().DownloadArchivoAsunto(Asunto);
                    Tuple<bool, byte[]> _resultado = e.Result as Tuple<bool, byte[]>;
                    if (!_resultado.Item1)
                        throw new Exception(DTOWDXDespachoVilla.Constantes.ConstantesComunes.ERROR_GENERICO);
                    elemento.Archivo = _resultado.Item2;

                }
            }

            private void DescargaAsuntoArchivoCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    foreach(VMAsuntos file in viewModelPadre.ListadoAsuntos.Where(x=> x.IsSelected))
                    {
                        string destPath = Path + @"\Documentos Asuntos DespachoVilla\"+file.Email;
                        if (!Directory.Exists(destPath))
                        {
                            Directory.CreateDirectory(destPath);
                        }
                        string ArchivoDestino = System.IO.Path.Combine(destPath, file.NombreArchivo);
                        using(FileStream stream = new FileStream(ArchivoDestino, FileMode.Create, FileAccess.ReadWrite))
                        {
                            stream.Write(file.Archivo, 0, file.Archivo.Length);
                        }
                        file.Archivo = null;
                    }
                    MessageBox.Show("Sus archivos se han guardado con éxito en la carpeta Mis Documentos");
                }
                else
                {
                    MessageBox.Show(String.Format("Ha ocurrido el siguiente error: {0}", e.Error.Message.ToString()));
                }
            }
        }
    }
}
