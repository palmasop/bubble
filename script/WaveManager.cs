using Godot;
using System.Collections.Generic;

[Tool]
public partial class WaveManager : Node
{
	[Export] public NodePath[] waveSpawners; // Array of paths to EnemySpawner nodes
	[Export] public float waveInterval = 10.0f; // Time between waves

	private int _currentWave = 0;
	private float _waveTimer = 0.0f;

	public override void _Process(double delta)
	{
		_waveTimer += (float)delta;

		if (_waveTimer >= waveInterval)
		{
			StartNextWave();
			_waveTimer = 0.0f; // Reset the wave timer
		}
	}

	private void StartNextWave()
	{
		if (_currentWave >= waveSpawners.Length) return; // No more waves

		var spawner = GetNode<EnemySpawner>(waveSpawners[_currentWave]);
		spawner.StartSpawning();
		_currentWave++;
	}
}
