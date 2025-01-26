using Godot;

public partial class EnemyBoss : Enemy
{
    [Export] PackedScene bullet;
    [Export] float offset = 200;
    [Export] GameOverMenu gameOverMenu;
    [Export] float ability1Cooldown = 5;
    [Export] float ability2Cooldown = 3;

    float _ability1Cooldown = 0;
    float _ability2Cooldown = 0;

    public override void _Ready()
    {
        base._Ready();
        health.OnDie += () => gameOverMenu.Show();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Engine.IsEditorHint())
            return;

        if (_ability1Cooldown > 0)
        {
            _ability1Cooldown -= (float)delta;
        }
        else
        {
            Ability1();
            _ability1Cooldown = ability1Cooldown;
        }

        if (_ability2Cooldown > 0)
        {
            _ability2Cooldown -= (float)delta;
        }
        else
        {
            Ability2();
            _ability2Cooldown = ability2Cooldown;
        }

    }

    private void Ability1()
    {
        GD.Print("Ability 1");
        var player = Player.GetPlayer();
        if (player == null)
            return;

        var bulletCount = GD.RandRange(1, 3);

        for (int i = 0; i < bulletCount; i++)
        {
            var bulletInstance = bullet.Instantiate<DamageBullet>();

            var randomOffset = new Vector2(
                (float)GD.RandRange(-offset, offset),
                (float)GD.RandRange(-offset, offset)
            );

            var targetPos = player.GlobalPosition + randomOffset;
            var direction = (targetPos - GlobalPosition).Normalized();

            bulletInstance.Init(this, 10, 800, 1, 10, null, direction);
            bulletInstance.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(bulletInstance);
        }
    }

    void Ability2()
    {
        GD.Print("Ability 2");
        var player = Player.GetPlayer();
        if (player == null)
            return;

        var bulletCount = GD.RandRange(2, 4);

        for (int i = 0; i < bulletCount; i++)
        {
            var bulletInstance = bullet.Instantiate<DamageBullet>();

            var randomOffset = new Vector2(
                (float)GD.RandRange(-offset, offset),
0
            );

            var targetPos = player.GlobalPosition + randomOffset;
            var direction = (targetPos - GlobalPosition).Normalized();

            bulletInstance.Init(this, 10, 800, 1, 10, null, Vector2.Down);
            bulletInstance.GlobalPosition = GlobalPosition + randomOffset;
            GetTree().Root.AddChild(bulletInstance);
        }
    }

    void Ability3()
    {

    }
}