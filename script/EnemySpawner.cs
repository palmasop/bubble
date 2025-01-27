using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene[] enemyScenes; // List of enemy scenes to spawn
	[Export] public float spawnInterval = 2.0f; // Time between spawns
	[Export] public int maxEnemies = 10; // Maximum number of enemies to spawn

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

	bool SpawnAble () => _spawnedEnemiesCount < maxEnemies || maxEnemies == -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint() || !_isSpawning) return;

		_timer += (float)delta;

		if (_timer >= spawnInterval && SpawnAble())
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
		if (enemyScenes.Length == 0 || !SpawnAble()) return; // Ensure there are enemy scenes to spawn and maxEnemies is not reached

		// Randomly select an enemy scene from the list
		var randomIndex = GD.Randi() % (int)enemyScenes.Length; // Get a random index
		var enemyScene = enemyScenes[randomIndex]; // Select the enemy scene
		var enemy = (Enemy)enemyScene.Instantiate();
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
