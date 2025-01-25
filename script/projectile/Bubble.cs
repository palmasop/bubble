using Godot;
using System;

public partial class Bubble : Projectile
{
    [Signal] public delegate void OnCaptureEventHandler(Node2D body);
    [Signal] public delegate void OnReleaseCapturedEventHandler(Node2D body);
    [Export] float floatSpeed = 100f;
    [Export] float slowTime = 1;
    Vector2 targetV;

    bool captureAble = true;

    Node2D captured = null;

    public void EnableCapture(bool enable) => captureAble = enable;

    public override void Init(float lifetime, float speed, float scale, int damage, PackedScene display)
    {
        base.Init(lifetime, speed, scale, damage, display);
        targetV = velocity / 10;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (captured != null)
            return;
        velocity = velocity.Slerp(targetV, slowTime * (float)delta);
    }

    public override void Explode()
    {
        ReleaseCaptured();
        base.Explode();
    }

    protected override void HandleOnHit(Area2D area)
    {
        base.HandleOnHit(area);
        if (area == captured)
            return;
        if (!HandleHitBubble(area))
        {
            HandleHitKnockback(area);
            HandleHit(area);
        }
    }

    bool HandleHitBubble(Area2D area)
    {
        if (area is not Bubble bubble)
            return false;

        bubble.Explode();
        Explode();
        return true;
    }


    void HandleHitKnockback(Area2D area)
    {
        var node = area.GetParent<Node2D>();

        if (node == null)
            return;

        var knockback = captured == null && captureAble ? 200 : 500;

        if (node is IKnockbackable knockbackable)
        {
            Vector2 knockbackForce = (node.GlobalPosition - GlobalPosition).Normalized() * knockback * Scale.X / 2;
            knockbackable.Knockback(knockbackForce);
        }
    }

    void HandleHit(Area2D area)
    {
        var node = area.GetParent<Node2D>();

        if (node is not IDamageable damageable)
            return;

        base.HandleOnHit(area);

        if (captured != null || !captureAble)
            return;

        if (node is ICapturable capturable && capturable.Capturable && Scale.X > capturable.minCaptureSize)
        {
            Capture(capturable);
            return;
        }

        damageable.TakeDamage(damage);
        Explode();
    }

    void Capture(ICapturable capturable)
    {
        if (capturable is not Node2D node)
            return;
        capturable.OnCapture();
        EmitSignal(SignalName.OnCapture, node);
        captured = node;
        velocity = Vector2.Up * floatSpeed;
    }

    void ReleaseCaptured()
    {
        if (captured == null || !IsInstanceValid(captured))
            return;

        if (captured.GetParent() is not ICapturable capturable || capturable is not Node2D node)
            return;

        capturable.OnReleaseCapture();
        EmitSignal(SignalName.OnReleaseCaptured, node);

        captured = null;
    }

    public override void _ExitTree()
    {
        ReleaseCaptured();
    }
}
