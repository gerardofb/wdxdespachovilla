using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WDXDespachoVillaAplicativo.ViewModel
{
    public class VMBase : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(PropertyName))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        
    }
    public class CommandObjeto : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public virtual void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
