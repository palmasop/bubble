using Godot;
using System;

public partial class Bubble : RigidBody2D
{
    [Export]
    public float Lifetime { get; set; } = 3.0f;  // How long the bubble exists before self-destructing
    
    private float _timeAlive = 0.0f;

    public override void _Ready()
    {
        GravityScale = 0.0f;  // Disable gravity so it moves straight
        LinearDamp = 0.0f;    // No air resistance
    }

    public void Init(float lifetime, float speed, float scale)
    {
        GD.Print(lifetime, " ", speed, " ", scale);  // Debug print to verify values
        // Set properties passed from the gun
        Lifetime = lifetime;
        LinearVelocity = Vector2.Up * speed;
        Scale = Vector2.One * scale;
        
        if (GetNode<CollisionShape2D>("CollisionShape2D") is CollisionShape2D collision)
        {
            collision.Scale = Vector2.One * scale;
        }
    }

    public override void _Process(double delta)
    {
        // Handle lifetime and cleanup
        _timeAlive += (float)delta;
        if (_timeAlive >= Lifetime)
        {
            // QueueFree();  // Remove the bubble when lifetime is exceeded
        }
    }

    public void OnBodyEntered(Node2D body)
    {
        // When the bubble hits something, destroy it
        QueueFree();
    }
}
