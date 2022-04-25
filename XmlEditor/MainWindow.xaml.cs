using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Win32;
using XmlEditor.Annotations;
using XmlEditor.ViewModel;

namespace XmlEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<XmlNodeViewModel> _xmlNodes;
        private XmlNodeViewModel _selectedXmlNode;
        private string _windowTitle;

        public MainWindow()
        {
            XmlNodes = new ObservableCollection<XmlNodeViewModel>();

            SetTitle();

            InitializeComponent();

            NodeEditor.OnSave += (_, _) =>
            {
                SelectedXmlNode?.UpdateFromElement();
            };
        }

        public string File { get; set; }

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

        private bool TryOpenFile(string file)
        {
            try
            {
                var txt = System.IO.File.ReadAllText(file);

                if (string.IsNullOrWhiteSpace(txt))
                {
                    return true;
                }

                var xml = XElement.Parse(txt);

                XmlNodes.Add(new XmlNodeViewModel(xml));
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return true;
        }

        private void SetTitle()
        {
            WindowTitle = string.IsNullOrWhiteSpace(File) ? "XmlEditor" : $"XmlEditor - {File}";
        }

        private void MiOpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (!string.IsNullOrWhiteSpace(File))
            {
                ofd.FileName = File;
            }

            if (ofd.ShowDialog() == true)
            {
                File = ofd.FileName;

                if (!TryOpenFile(File)) File = string.Empty;

                SetTitle();

                XmlNodes = new ObservableCollection<XmlNodeViewModel>();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MiNewFile_OnClick(object sender, RoutedEventArgs e)
        {

            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == true)
            {
                File = sfd.FileName;

                var fs = System.IO.File.Create(File);

                fs.Close();
                fs.Dispose();

                XmlNodes = new ObservableCollection<XmlNodeViewModel>();
            }
        }

        private void MiNewNode_OnClick(object sender, RoutedEventArgs e)
        {
            

            XElement newElement = new XElement("New", "New Content");
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
        }

        private void MiSaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(File))
            {
                return;
            }

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
                    XElement newXel = new XElement("Content");

                    foreach (var xel in xels)
                    {
                        newXel.Add(xel);
                    }

                    content = newXel.ToString();
                }

                using (FileStream fs = new FileStream(File, FileMode.Truncate, FileAccess.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is XmlNodeViewModel nvm)
            {
                SelectedXmlNode = nvm;
                NodeEditor.ViewModel = SelectedXmlNode.Clone();
            }
        }
    }
}
