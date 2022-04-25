using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using XmlEditor.Annotations;
using XmlEditor.Commands;
using XmlEditor.ViewModel;

namespace XmlEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        public static App Instance => (App)App.Current;

        public bool IsOpen => !string.IsNullOrWhiteSpace(FilePath);

        private XmlNodeViewModel _selectedXmlNode;
        private ObservableCollection<XmlNodeViewModel> _xmlNodes;
        public string FilePath;

        public event PropertyChangedEventHandler? PropertyChanged;

        public App()
        {
            CloseFileCommand = new CloseFileCommand();
            ExitCommand = new ExitCommand();
            NewFileCommand = new NewFileCommand();
            OpenFileCommand = new OpenFileCommand();
            SaveFileCommand = new SaveFileCommand();
        }

        public CloseFileCommand CloseFileCommand { get; set; }
        public ExitCommand ExitCommand { get; set; }
        public NewFileCommand NewFileCommand { get; set; }
        public OpenFileCommand OpenFileCommand { get; set; }
        public SaveFileCommand SaveFileCommand { get; set; }

        public ObservableCollection<XmlNodeViewModel> XmlNodes
        {
            get => _xmlNodes;
            set
            {
                if (Equals(value, _xmlNodes)) return;
                _xmlNodes = value;
                OnPropertyChanged();
            }
        }

        public XmlNodeViewModel SelectedXmlNode
        {
            get => _selectedXmlNode;
            set
            {
                if (Equals(value, _selectedXmlNode)) return;
                _selectedXmlNode = value;
                OnPropertyChanged();
            }
        }

        public void OpenFile(string filepath)
        {
            try
            {
                var txt = System.IO.File.ReadAllText(filepath);

                if (!string.IsNullOrWhiteSpace(txt))
                {
                    var xml = XElement.Parse(txt);

                    XmlNodes = new ObservableCollection<XmlNodeViewModel>();
                    SelectedXmlNode = null;

                    XmlNodes.Add(new XmlNodeViewModel(xml));
                }

                FilePath = filepath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Current.MainWindow, ex.Message);
            }
        }

        public void NewFile(string filepath)
        {
            var fs = System.IO.File.Create(filepath);
            fs.Dispose();

            OpenFile(filepath);
        }

        public void SaveFile()
        {
            try
            {
                var xels = XmlNodes.Select(x => x.Element);

                string content = null;

                if (xels.Count() == 1)
                {
                    content = xels.First().ToString();
                }
                else
                {
                    var newXel = new XElement("Content");

                    foreach (var xel in xels) newXel.Add(xel);

                    content = newXel.ToString();
                }

                using (var fs = new FileStream(FilePath, FileMode.Truncate, FileAccess.ReadWrite))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Current.MainWindow, ex.Message);
            }
        }

        public void CloseFile()
        {
            FilePath = null;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static bool PromptUnsavedChanges()
        {
            var result = MessageBox.Show(Current.MainWindow, "Unsaved changes will be lost", "Closing", MessageBoxButton.OKCancel);

            return result == MessageBoxResult.OK;
        }
    }
}
