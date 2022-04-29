using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XmlEditor.ViewModel;

namespace XmlEditor.Commands
{
    public class DeleteNodeCommand : ICommand

    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return parameter is XmlNodeViewModel;
        }

        public void Execute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public void RaiseCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
