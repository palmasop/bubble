using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class Enemy : CharacterBody2D, IDamageable
{
    public static readonly List<Enemy> Enemies = new();

    public static void KillAll()
    {
        foreach (var enemy in Enemies)
            enemy.health.Die();
    }

    [Signal] public delegate void OnEnemyDieEventHandler(Enemy body);

    [Export] public EnemyType type;
    [Export] protected PackedScene[] drops = Array.Empty<PackedScene>();

    [Export] protected int attackDamage;
    [Export] PackedScene dieEffect;

    [Export] protected bool debug;

    RandomNumberGenerator rng = new();
    public EnemyAI enemyAI { get; private set; }
    public Health health { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Enemies.Add(this);

        enemyAI = GetNodeOrNull<EnemyAI>("EnemyAI");
        health = GetNodeOrNull<Health>("Health");
        health.OnDie += Die;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Enemies.Remove(this);
        health.OnDie -= Die;
    }

    public void TakeDamage(int damage) { health.TakeDamage(damage); }

    public void Die()
    {
        DieEffect();
        CallDeferred(nameof(Drop));
        QueueFree();
    }

    void DieEffect()
    {
        var node = (Node2D)dieEffect.Instantiate();
        node.GlobalPosition = GlobalPosition;
        GetTree().CurrentScene.AddChild(node);
    }

    void Drop()
    {
        if (drops.Length == 0 || rng.Randf() > .15)
            return;

        var dropObj = drops[rng.RandiRange(0, drops.Length - 1)];
        var node = (Node2D)dropObj.Instantiate();
        node.GlobalPosition = GlobalPosition;
        GetTree().CurrentScene.AddChild(node);
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (GetNodeOrNull<Health>("Health") == null)
            warnings.Add("Health node is missing");

        return warnings.ToArray();
    }
}