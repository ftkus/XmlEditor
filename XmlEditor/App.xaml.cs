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
        private string _windowTitle;
        private string _searchFilter;


        public event PropertyChangedEventHandler? PropertyChanged;

        public App()
        {
            CloseFileCommand = new CloseFileCommand();
            ExitCommand = new ExitCommand();
            NewFileCommand = new NewFileCommand();
            OpenFileCommand = new OpenFileCommand();
            SaveFileCommand = new SaveFileCommand();
        }

        public string FilePath { get; private set; }

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

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                if (value == _searchFilter) return;
                _searchFilter = value;
                OnPropertyChanged();

                ApplySearchFilter(value);
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (value == _windowTitle) return;
                _windowTitle = value;
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

            SetTitle();
        }

        public void NewFile(string filepath)
        {
            var fs = System.IO.File.Create(filepath);
            fs.Dispose();

            OpenFile(filepath);
        }

        public void NewNode()
        {
            var newElement = new XElement("New", string.Empty);
            var newNode = new XmlNodeViewModel(newElement);

            if (SelectedXmlNode is null)
            {
                XmlNodes.Add(newNode);
            }
            else
            {
                SelectedXmlNode.Element.Add(newElement);
                SelectedXmlNode.IsExpanded = true;
                SelectedXmlNode.UpdateFromElement();
            }

            SelectedXmlNode = newNode;
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

            SetTitle();
        }

        public void CloseFile()
        {
            FilePath = null;

            SetTitle();
        }

        private void ApplySearchFilter(string searchFilter)
        {
            foreach (var xmlNode in XmlNodes)
            {
                xmlNode.ApplySearchFilter(searchFilter);
            }
        }

        private void SetTitle()
        {
            WindowTitle = string.IsNullOrWhiteSpace(FilePath) ? "XmlEditor" : $"XmlEditor - {FilePath}";
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
