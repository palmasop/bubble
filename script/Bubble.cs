using Godot;
using System;

public partial class Bubble : Projectile
{
    [Export] float floatSpeed = 100f;

    Node2D captured = null;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (captured != null)
        {
            if (IsInstanceValid(captured) && !captured.IsQueuedForDeletion())
            {
                captured.GlobalPosition = GlobalPosition;
                velocity = Vector2.Up * floatSpeed;
            }
            else
            {
                captured = null;
                QueueFree();
            }
        }
    }

    protected override void Explode()
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

        GD.Print(node?.Name, " ", captured?.Name);

        if (node == null || captured == null)
            return;

        if (node is IKnockbackable knockbackable)
        {
            Vector2 knockbackForce = (node.GlobalPosition - captured.GlobalPosition).Normalized() * 500;
            knockbackable.Knockback(knockbackForce);
        }
    }

    void HandleHit(Area2D area)
    {
        var node = area.GetParent<Node2D>();

        if (node is not IDamageable damageable)
            return;

        base.HandleOnHit(area);

        if (captured != null)
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
        captured = node;
    }

    void ReleaseCaptured()
    {
        if (captured?.GetParent() is ICapturable capturable)
            capturable.OnReleaseCapture();
    }

    public override void _ExitTree()
    {
        ReleaseCaptured();
    }
}
