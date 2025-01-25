using Godot;

[Tool]
[GlobalClass]
public partial class BubbleSettings : Resource
{
    public float ShootSpeed = 400.0f;
    [Export] public PackedScene displayGFX;
    [Export] public int bullet = 5;
    [Export] public float BulletLifetime = 3.0f;

    [Export] public float MinBulletScale = 0.5f;

    [Export] public float MaxBulletScale = 3.0f;

    [Export] public float ChargeRate = 2.0f;
    [Export] public int damage = 70;
    [Export] public bool isChargable = true;
}
