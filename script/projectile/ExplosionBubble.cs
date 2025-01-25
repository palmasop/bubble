using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class ExplosionBubble : Projectile
{
    [Export]
    public float ExplosionRadius
    {
        get => _explosionRadius;
        set
        {
            _explosionRadius = value;
            QueueRedraw();
        }
    }

    [Export]
    public Color DebugColor = new Color(1, 0, 0, 0.5f);

    private float _explosionRadius = 100f;

    Area2D _explosionArea;

    List<IDamageable> _targets = new();

    public override void _Ready()
    {
        base._Ready();
        SetupExplosionArea();
    }

    private void SetupExplosionArea()
    {
        _explosionArea = new Area2D();

        var collisionShape = new CollisionShape2D
        {
            Shape = new CircleShape2D { Radius = ExplosionRadius }
        };
        _explosionArea.AddChild(collisionShape);

        AddChild(_explosionArea);
        _explosionArea.GlobalPosition = GlobalPosition;
        _explosionArea.BodyEntered += AddTarget;
        _explosionArea.BodyExited += RemoveTarget;
        _explosionArea.AreaEntered += AddTarget;
        _explosionArea.AreaExited += RemoveTarget;
    }

    private void AddTarget(Node2D body)
    {
        if (body.GetParent() is not IDamageable damageable)
            return;
        _targets.Add(damageable);
    }

    private void RemoveTarget(Node2D body)
    {
        if (body.GetParent() is not IDamageable damageable)
            return;
        _targets.Remove(damageable);
    }

    private void DetectAndApplyDamage()
    {
        foreach (var target in _targets)
            target.TakeDamage(damage);
    }

    protected override void HandleOnHit(Area2D area)
    {
        if (area.GetParent() is not IDamageable damageable)
            return;

        DetectAndApplyDamage();
        base.HandleOnHit(area);
        Explode();
    }

    public override void _Draw()
    {
        if (Engine.IsEditorHint())
            DrawCircle(Vector2.Zero, ExplosionRadius, DebugColor);
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        base._Process(delta);
    }
}
