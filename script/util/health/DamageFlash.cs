using Godot;

public partial class DamageFlash : Node
{
    [Export] Health health;
    [Export] ColorRect flashOverlay;
    [Export] Color flashColor = new(1, 0, 0, 0.5f);
    [Export] float flashDuration = 0.1f;
    [Export] float fadeOutDuration = 0.5f;

    Tween tween;

    public override void _Ready()
    {
        flashOverlay.Visible = false;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _ExitTree() => health.OnTakeDamage -= OnTakeDamage;
    public override void _EnterTree() => health.OnTakeDamage += OnTakeDamage;
    void OnTakeDamage(int damageAmount) => FlashEffect();

    void FlashEffect()
    {
        Tween tween = GetTree().CreateTween();
        flashOverlay.Color = flashColor;
        flashOverlay.Visible = true;
        flashOverlay.Modulate = new Color(1, 1, 1, .5f);

        tween.TweenProperty(flashOverlay, "modulate:a", 0.0f, fadeOutDuration).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        tween.TweenCallback(Callable.From(() => flashOverlay.Visible = false));
    }
}