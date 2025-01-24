using Godot;

public partial class HealthBar : ProgressBar
{
    [Export] Health health;

    void OnUpdateHealth(int currentHealth) { Value = currentHealth; }

    public override void _Ready() {
        MaxValue = health.maxHealth;
        Value = health.currentHealth;
        health.OnChangeHealth += OnUpdateHealth;
    }

    public override void _ExitTree() {
        base._ExitTree();
        health.OnChangeHealth -= OnUpdateHealth;
    }
}