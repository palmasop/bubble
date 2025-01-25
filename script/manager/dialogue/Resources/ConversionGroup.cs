using System;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class ConversionGroup : Resource
{
    // [Export] string _id = Guid.NewGuid().ToString();
    [Export] public string id = "Fake Name";
    [Export] public Texture2D backgroundImage;  // Add this line
    // [Export] public string name = "Sample Name";
    [Export] public Conversion[] conversions;

    // public override Array<Dictionary> _GetPropertyList() {
    //     var propertyList = base._GetPropertyList() ?? new Array<Dictionary>();
    //
    //     var idProp = new Dictionary { { "name", "id" }, { "type", (int)Variant.Type.String }, { "usage", (int)(PropertyUsageFlags.ReadOnly | PropertyUsageFlags.Editor) } };
    //     propertyList.Add(idProp);
    //
    //     return propertyList;
    // }
}
