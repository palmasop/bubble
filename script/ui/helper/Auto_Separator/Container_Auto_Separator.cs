using Godot;
using System;
using System.Linq;

public partial class Container_Auto_Separator : Container
{
	public static void AddSeparatorsBetweenChildren(Container container, Func<Separator> createSeparator, Color? separatorColor = null)
	{
		var children = container.GetChildren().ToList();

		foreach (var child in children)
			container.RemoveChild(child);

		for (int i = 0; i < children.Count; i++)
		{
			container.AddChild(children[i]);
			if (i < children.Count - 1)
			{
				Separator separator = createSeparator();
				container.AddChild(separator);
			}
		}
	}

	public static void AddChildWithSeparator(Container container, Control newChild, Func<Separator> createSeparator, Color? separatorColor = null)
	{
		if (container.GetChildCount() > 0)
			container.AddChild(createSeparator());
		container.AddChild(newChild);
	}
}
