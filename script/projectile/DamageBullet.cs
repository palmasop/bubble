using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class DamageBullet : Projectile
{
    [Export]
    public Color DebugColor = new Color(1, 0, 0, 0.5f);
    [Export]
    public float DelayTime = 0.5f;  // Time before the bullet starts moving

    [Export] Node2D GFX;

    private Vector2 initialV;
    private bool isDelaying = true;

    private float _delayTimer = 0f;

    public override void Init(Node2D owner, float lifetime, float speed, float scale, int damage, PackedScene display, Vector2 direction)
    {
        base.Init(owner, lifetime, speed, scale, damage, display, direction);
        initialV = direction * speed;
        velocity = Vector2.Zero;
        _delayTimer = 0f;
        GFX.Rotation = direction.Angle();
    }


    public override void Init(Node2D owner, float lifetime, float speed, float scale, int damage, PackedScene display)
    {
        base.Init(owner, lifetime, speed, scale, damage, display);
    }

    private void OnDelayComplete()
    {
        isDelaying = false;
        velocity = initialV;
    }

    protected override void HandleOnHit(Area2D area)
    {
        if (IsHitOwner(area))
            return;

        if (area.GetParent() is not IDamageable damageable)
            return;

        if (area.GetParent() is not Player player)
            return;

        base.HandleOnHit(area);
        damageable.TakeDamage(damage);
    }

    public override void _Draw()
    {
        if (!isDelaying)
            return;
        DrawLine(-initialV * 200f, initialV * 200f, DebugColor, 20.0f);
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        if (isDelaying)
        {
            QueueRedraw();
            _delayTimer += (float)delta;
            if (_delayTimer >= DelayTime)
                OnDelayComplete();
        }

        QueueRedraw();
        base._Process(delta);
    }
}
