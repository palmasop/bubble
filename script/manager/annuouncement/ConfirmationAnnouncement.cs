using System;
using Godot;

public partial class ConfirmationAnnouncement : UI_Menu
{
    [Export] Control container;
    [Export] Label label;

    [Export] Button confirmBt;
    [Export] Button closeBt;

    void UnPause()
    {
        GetTree().Paused = false;
        ProcessMode = ProcessModeEnum.Inherit;
    }

    public override void ToggleMenu(bool? visible = null, Control target = null)
    {
        base.ToggleMenu(visible, target);
        if (Visible && visible == false)
        {
            GD.Print("QueueFree");
            QueueFree();
        }
    }

    void Open()
    {
        ToggleMenu(true, container);
    }

    public void ShowAnnouncement(string message, Action onConfirm)
    {
        void onPress()
        {
            UnPause();
            onConfirm?.Invoke();
            QueueFree();
        }

        confirmBt.Pressed += onPress;
        label.Text = message;
        Open();
    }
}