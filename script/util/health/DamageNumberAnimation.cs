using Godot;

[Tool]
public partial class DamageNumberAnimation : Node2D
{
    [Export] Health health;
    [Export] Color textColor = new Color(1, 0, 0);
    [Export] float floatSpeed = 50.0f;
    Vector2 _spawnAreaMin = new Vector2(-10, -10);
    Vector2 _spawnAreaMax = new Vector2(10, 10);

    [Export]
    public Vector2 spawnAreaMin
    {
        get => _spawnAreaMin;
        set
        {
            _spawnAreaMin = value;
            QueueRedraw(); // Request a redraw when property changes
        }
    }

    [Export]
    public Vector2 spawnAreaMax
    {
        get => _spawnAreaMax;
        set
        {
            _spawnAreaMax = value;
            QueueRedraw(); // Request a redraw when property changes
        }
    }
    [Export] float fadeDuration = 0.5f;
    [Export] float fontSize = 1;
    [Export] PackedScene label;

    Vector2 velocity;

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        health.OnTakeDamage += Initialize;
    }

    public void Initialize(int damageAmount)
    {
        float randomAngle = GD.Randf() * Mathf.Pi * 2;
        var direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        velocity = direction * (float)Mathf.Lerp(floatSpeed * .9, floatSpeed * 1.1, GD.Randf());

        var position = GlobalPosition + new Vector2(
            GD.Randf() * (spawnAreaMax.X - spawnAreaMin.X) + spawnAreaMin.X,
            GD.Randf() * (spawnAreaMax.Y - spawnAreaMin.Y) + spawnAreaMin.Y
        );

        DamageNumber damageNumber = label.Instantiate<DamageNumber>();
        damageNumber.Init(velocity, textColor, fontSize, floatSpeed, fadeDuration, damageAmount.ToString(), GlobalPosition);
        GetTree().Root.AddChild(damageNumber);
    }

    public override void _Draw()
    {
        if (Engine.IsEditorHint())
            DrawRect(new Rect2(spawnAreaMin, spawnAreaMax - spawnAreaMin), new Color(1, 0, 0, 0.5f));
    }
}