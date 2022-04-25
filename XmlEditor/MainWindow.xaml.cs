using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;
using XmlEditor.Annotations;
using XmlEditor.ViewModel;

namespace XmlEditor;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private string _windowTitle;
    private XmlNodeViewModel _selectedXmlNode;
    private ObservableCollection<XmlNodeViewModel> _xmlNodes;
    private string _searchFilter;

    public MainWindow()
    {
        XmlNodes = new ObservableCollection<XmlNodeViewModel>();

        SetTitle();

        InitializeComponent();

        NodeEditor.OnSave += (_, _) => { SelectedXmlNode?.UpdateFromElement(); };
    }

    public App App => App.Instance;

    public string File { get; set; }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetTitle()
    {
        WindowTitle = string.IsNullOrWhiteSpace(File) ? "XmlEditor" : $"XmlEditor - {File}";
    }

    private void ApplySearchFilter(string searchFilter)
    {
        foreach (var xmlNode in XmlNodes)
        {
            xmlNode.ApplySearchFilter(searchFilter);
        }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void MiNewNode_OnClick(object sender, RoutedEventArgs e)
    {
        var newElement = new XElement("New", "New Content");
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