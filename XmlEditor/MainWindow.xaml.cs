using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public MainWindow()
        {
            XmlNodes = new ObservableCollection<XmlNodeViewModel>();

            InitializeComponent();

            NodeEditor.OnSave += (_, _) =>
            {
                SelectedXmlNode?.UpdateFromElement();
            };
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

        private void MiOpen_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MiNew_OnClick(object sender, RoutedEventArgs e)
        {
            XmlNodes = new ObservableCollection<XmlNodeViewModel>();
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
