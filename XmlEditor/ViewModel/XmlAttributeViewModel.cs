using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents.DocumentStructures;
using XmlEditor.Annotations;

namespace XmlEditor.ViewModel;

public class XmlAttributeViewModel : INotifyPropertyChanged
{
    private string _name;
    private string _value;

    public XmlAttributeViewModel()
    {
        //Default ctor
        Name = string.Empty;
        Name = string.Empty;
    }

    public XmlAttributeViewModel(string name, object value)
    {
        Name = name;
        Value = value.ToString();
    }

    public string Value
    {
        get => _value;
        set
        {
            if (Equals(value, _value)) return;
            _value = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}