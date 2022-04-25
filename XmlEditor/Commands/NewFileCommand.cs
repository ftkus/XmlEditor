using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlEditor.Commands
{
    public class NewFileCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (App.Instance.IsOpen)
            {
                if (!App.PromptUnsavedChanges()) { return; }
            }

            var sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == true)
            {
                App.Instance.NewFile(sfd.FileName);
            }
        }
    }
}
