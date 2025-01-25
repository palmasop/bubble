using Godot;
using System;

public partial class Bubble : Area2D
{
    [Signal] public delegate void OnHitEventHandler(Enemy body);

    [Export] PackedScene explodeEffect;

    float lifetime { get; set; } = 3.0f;
    float floatSpeed = 100f;
    int damage;
    bool isCapture;

    float timeAlive = 0.0f;
    Vector2 velocity = Vector2.Zero;
    Enemy capturedEnemy = null;

   [Export] Node2D GFX;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public void Init(float lifetime, float speed, float scale, int damage, bool isCapture, PackedScene display)
    {
        velocity = Vector2.Up * speed;
        Scale = Vector2.One * scale;
        this.lifetime = lifetime;
        this.damage = damage;
        this.isCapture = isCapture;

        foreach(var child in GFX.GetChildren())
            child.QueueFree();
        var displayGFX = display.Instantiate<Node2D>();
        GFX.AddChild(displayGFX);
        displayGFX.GlobalPosition = GFX.GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if (capturedEnemy != null)
        {
            if (IsInstanceValid(capturedEnemy) && !capturedEnemy.IsQueuedForDeletion())
            {
                capturedEnemy.GlobalPosition = GlobalPosition;
                velocity = Vector2.Up * floatSpeed;
            }
            else
            {
                capturedEnemy = null;
                QueueFree();
            }
        }

        Position += velocity * (float)delta;

        timeAlive += (float)delta;
        if (timeAlive >= lifetime)
            Explode();
    }

    void Explode()
    {
        ReleaseCapturedEnemy();
        DieEffect();
        QueueFree();
    }

    void DieEffect()
    {
        var node = (Node2D)explodeEffect.Instantiate();
        node.GlobalPosition = GlobalPosition;
        node.Scale = Scale;
        GetTree().CurrentScene.AddChild(node);
    }

    public void OnAreaEntered(Area2D area)
    {
        if (!HandleHitBubble(area))
            HandleHitEnemey(area);
    }

    bool HandleHitBubble(Area2D area)
    {
        GD.Print(area.Name);
        if (area is not Bubble bubble)
            return false;
        GD.Print("is bubble");

        bubble.Explode();
        Explode();
        return true;
    }


    void HandleHitEnemey(Area2D area)
    {
        if (area.GetParent() is not Enemy enemy)
            return;

        EmitSignal(SignalName.OnHit, enemy);

        var enemySetting = BulletManager.Instance.GetEnemySettingsByEnemey(enemy.type);
        var isCapture = this.isCapture && Scale.X > enemySetting.minCaptureSize;

        if (!isCapture && !enemySetting.bubbleSettings.isCapturable)
        {
            enemy.TakeDamage(damage);
            Explode();
            return;
        }

        if (capturedEnemy != null)
        {
            Vector2 knockbackForce = (enemy.GlobalPosition - capturedEnemy.GlobalPosition).Normalized() * 1000;
            GD.Print(knockbackForce);
            enemy.Knockback(knockbackForce);
        }

        if (capturedEnemy == null)
            CaptureEnemy(enemy);
    }

    void CaptureEnemy(Enemy enemy)
    {
        capturedEnemy = enemy;

        enemy.enemyAI.disabledMovement = true;
        enemy.SetProcess(false);
        enemy.SetPhysicsProcess(false);

        foreach (var child in enemy.GetChildren())
        {
            if (child is Area2D col)
                col.Monitoring = false;
        }
    }

    void ReleaseCapturedEnemy()
    {
        if (capturedEnemy != null)
        {
            capturedEnemy.enemyAI.disabledMovement = false;
            capturedEnemy.SetProcess(true);
            capturedEnemy.SetPhysicsProcess(true);
            foreach (var child in capturedEnemy.GetChildren())
            {
                if (child is Area2D col)
                    col.Monitoring = true;
            }
            capturedEnemy = null;
        }
    }

    public override void _ExitTree()
    {
        ReleaseCapturedEnemy();
    }
}
