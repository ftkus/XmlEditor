using System;
using System.Collections.Generic;
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
using XmlEditor.Annotations;
using XmlEditor.ViewModel;

namespace XmlEditor.Views
{
    /// <summary>
    /// Interaction logic for XmlNodeEditor.xaml
    /// </summary>
    public partial class XmlNodeEditor : UserControl, INotifyPropertyChanged
    {
        private XmlNodeViewModel _viewModel;

        public event EventHandler OnSave;

        public XmlNodeEditor()
        {
            InitializeComponent();
        }

        public XmlNodeViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.UpdateElement();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, ex.Message);
                return;
            }

            OnSave?.Invoke(this, EventArgs.Empty);
        }
    }
}
