using Godot;
using System;

public partial class Bubble : Area2D
{
    [Export] public float Lifetime { get; set; } = 3.0f;  // How long the bubble exists before self-destructing
    [Export] private float floatSpeed = 100f; // Speed at which captured enemy floats up

    private float _timeAlive = 0.0f;
    private Vector2 _velocity = Vector2.Zero;  // Added to handle movement
    private Enemy _capturedEnemy = null;
    private Vector2 _originalEnemyPosition;

    public override void _Ready()
    {
        // Connect the area entered signal if not already connected in editor
        AreaEntered += OnAreaEntered;
    }

    public void Init(float lifetime, float speed, float scale)
    {
        GD.Print(lifetime, " ", speed, " ", scale);  // Debug print to verify values
        // Set properties passed from the gun
        Lifetime = lifetime;
        _velocity = Vector2.Up * speed;  // Store velocity for manual movement
        Scale = Vector2.One * scale;

        if (GetNode<CollisionShape2D>("CollisionShape2D") is CollisionShape2D collision)
        {
            collision.Scale = Vector2.One * scale;
        }
    }

    public override void _Process(double delta)
    {
        if (_capturedEnemy != null)
        {
            _capturedEnemy.GlobalPosition = GlobalPosition;

            _velocity = Vector2.Up * floatSpeed;
        }

        Position += _velocity * (float)delta;

        _timeAlive += (float)delta;
        if (_timeAlive >= Lifetime)
        {
            Explode();
        }
    }

    void Explode()
    {
        ReleaseCapturedEnemy();
        QueueFree();
    }

    public void OnAreaEntered(Area2D area)
    {
        GD.Print(area.Name);
        // Check if we hit an enemy and haven't captured one yet
        if (_capturedEnemy == null && area.GetParent() is Enemy enemy)
        {
            CaptureEnemy(enemy);
        }
    }

    public void OnBodyEntered(Node2D body)
    {
        GD.Print(body.Name);
        // Check if we hit an enemy and haven't captured one yet
        if (_capturedEnemy == null && body is Enemy enemy)
        {
            CaptureEnemy(enemy);
        }
    }

    void CaptureEnemy(Enemy enemy)
    {
        GD.Print(enemy.Name);

        _capturedEnemy = enemy;
        _originalEnemyPosition = enemy.GlobalPosition;

        enemy.enemyAI.disabledMovement = true;
        enemy.SetProcess(false);
        enemy.SetPhysicsProcess(false);
    }

    void ReleaseCapturedEnemy()
    {
        if (_capturedEnemy != null)
        {
            _capturedEnemy.enemyAI.disabledMovement = false;
            _capturedEnemy.SetProcess(true);
            _capturedEnemy.SetPhysicsProcess(true);
            _capturedEnemy = null;
        }
    }

    public override void _ExitTree()
    {
        ReleaseCapturedEnemy();
    }
}
