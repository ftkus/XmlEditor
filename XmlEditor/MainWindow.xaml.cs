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
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        NodeEditor.OnSave += (_, _) => { App.SelectedXmlNode?.UpdateFromElement(); };
    }

    public App App => App.Instance;

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is XmlNodeViewModel nvm)
        {
            App.SelectedXmlNode = nvm;
            NodeEditor.ViewModel = App.SelectedXmlNode.Clone();
        }
    }
}