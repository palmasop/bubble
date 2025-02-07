using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class EnemySpawner : Node2D
{
    [Export] public float nextInterval = 2.0f; // Time between spawns
    [Export] public int amount = 10; // Number of enemies to spawn
    [Export] public PackedScene enemyScene; // Single enemy scene to spawn

    [Export]
    public Vector2 spawnAreaMin
    {
        get => _spawnAreaMin;
        set
        {
            _spawnAreaMin = value;
            QueueRedraw(); // Request a redraw when property changes
        }
    }
    private Vector2 _spawnAreaMin = new Vector2(0, 0); // Minimum corner of the spawn area

    [Export]
    public Vector2 spawnAreaMax
    {
        get => _spawnAreaMax;
        set
        {
            _spawnAreaMax = value;
            QueueRedraw(); // Request a redraw when property changes
        }
    }
    private Vector2 _spawnAreaMax = new Vector2(800, 600); // Maximum corner of the spawn area

    private float _timer = 0.0f;
    private bool _isSpawning = false;
    private int _spawnedEnemiesCount = 0;

    bool SpawnAble() => _spawnedEnemiesCount < amount || amount == -1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() || !_isSpawning) return;

        _timer += (float)delta;

        if (_timer >= nextInterval && SpawnAble())
        {
            SpawnEnemy();
            _timer = 0.0f; // Reset the timer
        }
    }

    public void StartSpawning()
    {
        _isSpawning = true;
        _timer = 0.0f; // Reset the timer
    }

    private void SpawnEnemy()
    {
        if (enemyScene == null || !SpawnAble()) return; // Ensure there is an enemy scene to spawn and amount is not reached

        var enemy = (Node2D)enemyScene.Instantiate();

        // Set specific spawn position if available, otherwise use random position
        enemy.GlobalPosition = GetRandomSpawnPosition(); // Set a random spawn position

        GetTree().CurrentScene.AddChild(enemy);
        _spawnedEnemiesCount++; // Increment the spawned enemies count
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // Generate a random position within the defined spawn area
        float X = GD.Randf() * (spawnAreaMax.X - spawnAreaMin.X) + spawnAreaMin.X;
        float Y = GD.Randf() * (spawnAreaMax.Y - spawnAreaMin.Y) + spawnAreaMin.Y;
        return new Vector2(X, Y) + GlobalPosition; // Adjust by the spawner's global position
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return;

        DrawRect(new Rect2(spawnAreaMin, spawnAreaMax - spawnAreaMin), new Color(1, 0, 0, 0.5f)); // Semi-transparent red
    }
}
