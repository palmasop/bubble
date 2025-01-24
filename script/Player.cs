using System.Linq;
using Godot;
using System.Collections.Generic;

[Tool]
public partial class Player : CharacterBody2D, IDamageable
{
    public static readonly List<Player> Players = new();
    public static Player GetClosetPlayer(Vector2 position) => Players.MinBy(player => position.DistanceTo(player.GlobalPosition));
    public static Player GetPlayer() => Players.Count > 0 ? Players[0] : null;

    [Signal] public delegate void OnPlayerDieEventHandler(Player player);

    [Export] PackedScene dieEffect;

    Health health;
    Movement2D movement;

    public override void _EnterTree() => Players.Add(this);
    public override void _ExitTree() => Players.Remove(this);

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        health = GetNodeOrNull<Health>("Health");
        health.OnDie += () => EmitSignal(SignalName.OnPlayerDie, this);
        health.OnDie += Die;
    }

    void Die()
    {
        CameraShake.StartShake(this, 6, .32f);

        // var node = (Node2D)dieEffect.Instantiate();
        // node.GlobalPosition = GlobalPosition;
        // GetTree().CurrentScene.AddChild(node);
        Hide();
    }

    public void TakeDamage(int damage) { health.TakeDamage(damage); }
    public void Heal(int amount) { health.Heal(amount); }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (GetNodeOrNull<Health>("Health") == null)
            warnings.Add("Health node is missing");

        return warnings.ToArray();
    }
}