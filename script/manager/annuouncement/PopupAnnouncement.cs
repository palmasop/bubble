using Godot;

public partial class PopupAnnouncement : Control
{
    [Export] Label label;

    Tween tween;

    Tween GetTween()
    {
        if (tween == null || !tween.IsValid())
            return tween = CreateTween();
        return tween;
    }

    public async void ShowAnnouncement(string message, int size = -1, float displayTime = 2.0f)
    {
        label.Text = message;
        SetLabelFontSize(size);
        label.Modulate = new Color(1, 1, 1, 0);
        GetTween().TweenProperty(label, "modulate:a", 1.0f, 0.5f);
        await ToSignal(GetTween(), "finished");
        await ToSignal(GetTree().CreateTimer(displayTime), "timeout");
        GetTween().TweenProperty(label, "modulate:a", 0.0f, 0.5f);
        await ToSignal(GetTween(), "finished");
        QueueFree();
    }

    void SetLabelFontSize(int size)
    {
        if (size < 0) return;
        label.AddThemeFontSizeOverride("font_size", size);
    }
}