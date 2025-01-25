using Godot;

public partial class DamageNumber : Label
{
    [Export] private Color textColor = new Color(1, 0, 0);
    [Export] private float floatSpeed = 50.0f;
    [Export] private float fadeDuration = 0.5f;

    private Vector2 velocity;
    private float fadeTimer = 0f;
    public void Init(Vector2 velocity, Color textColor, float fontSize, float floatSpeed, float fadeDuration, string text, Vector2 startPosition)
    {
        this.velocity = velocity;
        this.textColor = textColor;
        Scale = new Vector2(fontSize, fontSize);
        this.floatSpeed = floatSpeed;
        this.fadeDuration = fadeDuration;
        Text = text;
        GlobalPosition = startPosition;
    }

    public override void _Ready()
    {
        Modulate = textColor;
        velocity = new Vector2(0, -floatSpeed);
    }

    public override void _Process(double delta)
    {
        Position += velocity * (float)delta;
        fadeTimer += (float)delta;
        if (fadeTimer >= fadeDuration)
            QueueFree();

        // Modulate.A -= (float)delta / fadeDuration;
        // if (Modulate.a <= 0)
        // QueueFree();
    }
}