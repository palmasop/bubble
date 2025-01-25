using Godot;
using System;

public partial class HBox_Auto_Separator : HBoxContainer
{
	[Export] Color separatorColor;

	public override void _Ready()
	{
		Container_Auto_Separator.AddSeparatorsBetweenChildren(this, () => new VSeparator(), separatorColor);
	}

	public void AddNewElement(Control newElement)
	{
		Container_Auto_Separator.AddChildWithSeparator(this, newElement, () => new VSeparator(), separatorColor);
	}
}