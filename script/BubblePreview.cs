using Godot;
using System;

public partial class BubblePreview : Node2D
{
    public void UpdatePreview(float scale)
    {
        Scale = Vector2.One * scale;
    }

    public void ChangePreview(PackedScene display){
        foreach(var child in GetChildren())
            child.QueueFree();
		
        var displayGFX = display.Instantiate<Node2D>();
		AddChild(displayGFX);
        displayGFX.GlobalPosition = GlobalPosition;
    }
}