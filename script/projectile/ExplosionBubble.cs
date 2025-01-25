using Godot;
using System;

public partial class ExplosionBubble : Projectile
{
    [Export] float explosionRadius = 100f;
    [Export] PackedScene explosionEffect;
    [Export] Color debugColor = new Color(1, 0, 0, 0.5f);

    public override void Explode()
    {
        base.Explode();

        if (explosionEffect != null)
        {
            var explosion = (Node2D)explosionEffect.Instantiate();
            explosion.GlobalPosition = GlobalPosition;
            GetParent().AddChild(explosion);
        }

        DetectDamageableBodies();
        QueueFree();
    }

    private void DetectDamageableBodies()
    {
        var explosionArea = new Area2D();
        var collisionShape = new CircleShape2D { Radius = explosionRadius };
        var shapeOwner = new CollisionShape2D();
        shapeOwner.Shape = collisionShape;
        explosionArea.AddChild(shapeOwner);

        GetParent().AddChild(explosionArea);
        explosionArea.GlobalPosition = GlobalPosition;

        var overlappingBodies = explosionArea.GetOverlappingBodies();

        foreach (var body in overlappingBodies)
        {
            if (body is IDamageable damageable)
            {
                damageable.TakeDamage(damage);
            }
        }

        explosionArea.QueueFree();
    }

    protected override void HandleOnHit(Area2D area)
    {
        base.HandleOnHit(area);
        Explode();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, explosionRadius, debugColor);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        Update();
    }
}
