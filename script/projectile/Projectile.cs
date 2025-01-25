using Godot;
using System;

public partial class Projectile : Area2D
{
    [Signal] public delegate void OnHitEventHandler(Area2D body);

    [Export] PackedScene explodeEffect;
    [Export] Node2D GFX;

    protected float lifetime { get; set; } = 3.0f;
    protected int damage;
    protected bool isCapture;

    float timeAlive = 0.0f;
    protected Vector2 velocity = Vector2.Zero;


    public override void _Ready()
    {
        AreaEntered += HandleOnHit;
    }

    public void Init(float lifetime, float speed, float scale, int damage, PackedScene display)
    {
        velocity = Vector2.Up * speed;
        Scale = Vector2.One * scale;
        this.lifetime = lifetime;
        this.damage = damage;

        foreach (var child in GFX.GetChildren())
            child.QueueFree();
        var displayGFX = display.Instantiate<Node2D>();
        GFX.AddChild(displayGFX);
        displayGFX.GlobalPosition = GFX.GlobalPosition;
    }

    public override void _Process(double delta)
    {
        Position += velocity * (float)delta;

        timeAlive += (float)delta;
        if (timeAlive >= lifetime)
            Explode();
    }

    void DieEffect()
    {
        var node = (Node2D)explodeEffect.Instantiate();
        node.GlobalPosition = GlobalPosition;
        node.Scale = Scale;
        GetTree().CurrentScene.AddChild(node);
    }

    protected virtual void Explode()
    {
        DieEffect();
        QueueFree();
    }

    protected virtual void HandleOnHit(Area2D area)
    {
        EmitSignal(SignalName.OnHit, area);
    }
}
