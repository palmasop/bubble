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
    protected Node2D owner;

    public override void _Ready()
    {
        AreaEntered += HandleOnHit;
    }

    public virtual void Init(Node2D owner, float lifetime, float speed, float scale, int damage, PackedScene display, Vector2 direction)
    {
        this.owner = owner;
        velocity = direction * speed;
        Scale = Vector2.One * scale;
        this.lifetime = lifetime;
        this.damage = damage;

        if (display != null)
        {
            foreach (var child in GFX.GetChildren())
                child.QueueFree();

            var displayGFX = display.Instantiate<Node2D>();
            GFX.AddChild(displayGFX);
            displayGFX.GlobalPosition = GFX.GlobalPosition;
        }
    }

    public virtual void Init(Node2D owner, float lifetime, float speed, float scale, int damage, PackedScene display)
    {
        Init(owner, lifetime, speed, scale, damage, display, Vector2.Up);
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
        if (node is CpuParticles2D particles)
        {
            particles.ScaleAmountMin *= Scale.X;
            particles.ScaleAmountMax *= Scale.X;
        }
        node.GlobalPosition = GlobalPosition;
        node.Scale = Scale;
        GetTree().CurrentScene.AddChild(node);
    }

    public virtual void Explode()
    {
        DieEffect();
        QueueFree();
    }

    protected virtual void HandleOnHit(Area2D area)
    {
        EmitSignal(SignalName.OnHit, area);
    }

    protected virtual bool IsHitOwner(Area2D area)
    {
        GD.Print(area.Name + " " + area.GetParent().Name + " " + owner.Name + " " + owner.GetParent().Name);
        return area == owner || area.GetParent() == owner || owner.GetParent() == area || owner.GetParent() == area.GetParent();
    }
}
