using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using XmlEditor.Annotations;
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

        public void Open(string filepath)
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
            }
            catch (Exception ex)
            {
                return;
            }

            FilePath = filepath;
        }

        public void Save()
        {

        }

        public void Close()
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
