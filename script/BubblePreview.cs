using Godot;
using System;

public partial class BubblePreview : Node2D
{
    public void UpdatePreview(float scale)
    {
        Scale = Vector2.One * scale;
    }

    public void Show()
    {
        Visible = true;
    }

    public void Hide()
    {
        Visible = false;
    }
}