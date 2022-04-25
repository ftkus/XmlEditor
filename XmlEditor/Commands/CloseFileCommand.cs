using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlEditor.Commands
{
    internal class CloseFileCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return App.Instance.IsOpen;
        }

        public void Execute(object? parameter)
        {
            if (App.Instance.IsOpen)
            {
                if (!App.PromptUnsavedChanges()) { return; }
            }

            App.Instance.CloseFile();
        }
    }
}
