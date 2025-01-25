using System;
using System.Linq;
using Godot;
using Godot.Collections;

public enum ConversionOptionType
{
    Default,
    Close,
    Action,
    Jump,
}

[Tool]
[GlobalClass]
public partial class ConversionOption : Resource
{
    public ConversionOptionType type
    {
        get => _type;
        set
        {
            _type = value;
            NotifyPropertyListChanged();
        }
    }
    ConversionOptionType _type = ConversionOptionType.Default;
    public string text = "Sample Text";
    public string action;
    public string jumpTo;

    public override Array<Dictionary> _GetPropertyList()
    {
        var propertyList = base._GetPropertyList() ?? new Array<Dictionary>();

        AddProperty(propertyList, "text", Variant.Type.String);
        AddProperty(propertyList, "type", Variant.Type.Int, PropertyHint.Enum, GenerateEnumHint<ConversionOptionType>());

        if (type == ConversionOptionType.Action)
            AddProperty(propertyList, "action", Variant.Type.String);
        if (type == ConversionOptionType.Jump)
            AddProperty(propertyList, "jumpTo", Variant.Type.String);

        return propertyList;
    }

    #region Helper

    void AddProperty(Array<Dictionary> propertyList, string name, Variant.Type type, PropertyHint hint = PropertyHint.None, string hintString = "")
    {
        var prop = new Dictionary { { "name", name }, { "type", (int)type }, { "usage", (int)(PropertyUsageFlags.Default | PropertyUsageFlags.Editor) } };

        if (hint != PropertyHint.None)
            prop["hint"] = (int)hint;

        if (!string.IsNullOrEmpty(hintString))
            prop["hint_string"] = hintString;

        propertyList.Add(prop);
    }

    void AddLabel(Array<Dictionary> propertyList, string labelText)
    {
        var labelProp = new Dictionary {
            { "name", labelText },
            { "type", (int)Variant.Type.String }, // Nil type to avoid appearing as a field
            { "usage", (int)(PropertyUsageFlags.Editor | PropertyUsageFlags.ReadOnly) },
        };

        propertyList.Add(labelProp);
    }

    string GenerateEnumHint<T>() where T : Enum { return string.Join(",", Enum.GetValues(typeof(T)).Cast<T>().Select(e => $"{e}:{Convert.ToInt32(e)}")); }

    #endregion
}