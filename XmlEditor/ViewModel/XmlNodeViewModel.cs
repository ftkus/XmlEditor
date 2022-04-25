using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using XmlEditor.Annotations;

namespace XmlEditor.ViewModel;

public class XmlNodeViewModel : INotifyPropertyChanged
{
    private ObservableCollection<XmlNodeViewModel> _children;
    private string _name;
    private string _payload;
    private bool _isExpanded;
    private bool _isSelected;
    private bool _canModifyPayload;
    private bool _isVisible;

    public XmlNodeViewModel(XElement element)
    {
        Element = element;

        IsVisible = true;

        UpdateFromElement();
    }

    public string Payload
    {
        get => _payload;
        set
        {
            if (value == _payload) return;
            _payload = value;
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

    public ObservableCollection<XmlNodeViewModel> Children
    {
        get => _children;
        set
        {
            if (Equals(value, _children)) return;
            _children = value;
            OnPropertyChanged();
        }
    }

    public XElement Element { get; }

    public List<XmlAttributeViewModel> Attributes { get; set; }

    public bool CanModifyPayload
    {
        get => _canModifyPayload;
        set
        {
            if (value == _canModifyPayload) return;
            _canModifyPayload = value;
            OnPropertyChanged();
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (value == _isVisible) return;
            _isVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (value == _isExpanded) return;
            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value == _isSelected) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public void UpdateFromElement()
    {
        Name = Element.Name.LocalName;

        Attributes = new List<XmlAttributeViewModel>();

        Attributes.AddRange(Element.Attributes().Select(a => new XmlAttributeViewModel(a.Name.LocalName, a.Value)));

        var children = Element.Elements().Select(x => new XmlNodeViewModel(x));

        Children = new ObservableCollection<XmlNodeViewModel>(children);

        CanModifyPayload = !Children.Any();

        if (CanModifyPayload) Payload = Element.Value;
    }

    public bool ApplySearchFilter(string searchFilter)
    {
        bool isMatchFound = false;

        foreach (var child in Children)
        {
            if (child.ApplySearchFilter(searchFilter))
            {
                isMatchFound = true;
            }
        }

        if (string.IsNullOrWhiteSpace(searchFilter))
        {
            IsVisible = true;
            return true;
        }

        if (!isMatchFound)
        {
            if (IsMatch(searchFilter, Name))
            {
                isMatchFound = true;
            }

            if (IsMatch(searchFilter, Payload))
            {
                isMatchFound = true;
            }
        }

        if (!isMatchFound)
        {
            foreach (var attribute in Attributes)
            {
                if (IsMatch(searchFilter, attribute.Name))
                {
                    isMatchFound = true;
                    break;
                }

                if (IsMatch(searchFilter, attribute.Value))
                {
                    isMatchFound = true;
                    break;
                }
            }
        }

        IsVisible = isMatchFound;
        return isMatchFound;
    }

    public void UpdateElement()
    {
        Element.Name = Name;

        if (!Children.Any()) Element.Value = Payload;

        Element.Attributes().Remove();

        foreach (var att in Attributes)
        {
            Element.SetAttributeValue(att.Name, att.Value);
        }
    }

    public IEnumerable<XmlNodeViewModel> GetAllChildren()
    {
        return Children.Concat(Children.SelectMany(c => c.GetAllChildren()));
    }

    public XmlNodeViewModel Clone()
    {
        XmlNodeViewModel retval = new XmlNodeViewModel(Element);

        return retval;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static bool IsMatch(string searchFilter, string field)
    {
        if (string.IsNullOrWhiteSpace(field)) return false;

        return field.ToLower().Contains(searchFilter.ToLower());
    }
}