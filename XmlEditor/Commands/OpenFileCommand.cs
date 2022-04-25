using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlEditor.Commands
{
    internal class OpenFileCommand : ICommand
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

            var ofd = new OpenFileDialog();

            if (!string.IsNullOrWhiteSpace(App.Instance.FilePath)) ofd.FileName = App.Instance.FilePath;

            if (ofd.ShowDialog() == true)
            {
                App.Instance.OpenFile(ofd.FileName);
            }
        }
    }
}
