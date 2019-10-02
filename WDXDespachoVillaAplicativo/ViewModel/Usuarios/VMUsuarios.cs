using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOWDXDespachoVilla;
using System.Windows.Input;
using System.Windows;
using System.Security.Cryptography;

namespace WDXDespachoVillaAplicativo.ViewModel.Usuarios
{
    public class VMUsuarios : VMBase
    {
        private string _User;
        private string _Password;
        private CommandUsuarios _Comando;
        public string User { get { return _User; } set { _User = value; OnPropertyChanged("User"); } }
        public string Password { get { return _Password; } set { _Password = value; OnPropertyChanged("Password"); } }
        public CommandUsuarios Comando
        {
            get { if (_Comando == null) _Comando = new CommandUsuarios(); return _Comando; }
            set { _Comando = value; }
        }
        public class CommandUsuarios : CommandObjeto
        {
            public override void Execute(object parameter)
            {
                if(parameter is VMUsuarios)
                {
                    PersonasDirectorio nuevaVentana = new PersonasDirectorio();
                    Application.Current.Windows.OfType<Login>().FirstOrDefault().Close();
                    nuevaVentana.Show();
                }
            }
        }
        public static string ComputarSha256(string RawData)
        {
            using (SHA256 Sha = SHA256.Create())
            {
                byte[] arreglo = Sha.ComputeHash(Encoding.UTF8.GetBytes(RawData));
                StringBuilder Sb = new StringBuilder();
                for (int i = 0; i < arreglo.Length; i++)
                {
                    Sb.Append(arreglo[i].ToString("X2"));
                }
                return Sb.ToString();
            }
        }
    }
}
