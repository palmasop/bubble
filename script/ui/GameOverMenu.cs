using Godot;

public partial class GameOverMenu : Control
{
    [Export] Button restartButton;

    public override void _Ready()
    {
        restartButton.Pressed += OnRestartPressed;
        Hide();
    }

    void OnRestartPressed()
    {
        GetTree().ReloadCurrentScene();
    }
}
