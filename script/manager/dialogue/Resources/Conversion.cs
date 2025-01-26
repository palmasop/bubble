using System;
using System.Linq;
using Godot;
using Godot.Collections;

public enum ConversionType
{
    Default,
    Close,
    Action,
    Jump,
    Option,
}

[Tool]
[GlobalClass]
public partial class Conversion : Resource
{
    public ConversionType type
    {
        get => _type;
        set
        {
            _type = value;
            NotifyPropertyListChanged();
        }
    }

    [Export] ConversionType _type = ConversionType.Default;
    [Export] public CharacterT character = CharacterT.Player;
    [Export] public string text = "Sample Text";
    [Export] public Texture2D backgroundImage;
    [Export] public Rect2 backgroundImageRect;
    [Export] public ConversionOption[] options;
    [Export] public string action;
    [Export] public string jumpTo;

    // public override void _ValidateProperty(Dictionary property) {
    //     void hideField(StringName name) {
    //         if (property["name"].AsStringName() != name)
    //             return;
    //         var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
    //         property["usage"] = (int)usage;
    //     }
    //
    //     if (type != ConversionType.Option)
    //         hideField(PropertyName.options);
    //     if (type != ConversionType.Action)
    //         hideField(PropertyName.action);
    //     if (type != ConversionType.Jump)
    //         hideField(PropertyName.jumpTo);
    //     if (property["name"].AsStringName() == PropertyName.type)
    //         NotifyPropertyListChanged();
    // }

    // public override Array<Dictionary> _GetPropertyList() {
    //     var propertyList = base._GetPropertyList() ?? new Array<Dictionary>();
    //
    //     AddProperty(propertyList, "character", Variant.Type.Int, PropertyHint.Enum, GenerateEnumHint<CharacterT>());
    //     AddProperty(propertyList, "text", Variant.Type.String);
    //     AddProperty(propertyList, "type", Variant.Type.Int, PropertyHint.Enum, GenerateEnumHint<ConversionType>());
    //
    //     if (type == ConversionType.Option)
    //         AddProperty(propertyList, "options", Variant.Type.Dictionary, PropertyHint.ResourceType, "Resource");
    //     if (type == ConversionType.Action)
    //         AddProperty(propertyList, "action", Variant.Type.String);
    //     if (type == ConversionType.Jump)
    //         AddProperty(propertyList, "jumpTo", Variant.Type.String);
    //
    //     return propertyList;
    // }

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
