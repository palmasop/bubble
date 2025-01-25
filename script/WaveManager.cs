using Godot;
using System.Collections.Generic;

[Tool]
public partial class WaveManager : Node2D
{
	[Export] Node2D waveBackground;
	[Export] public NodePath[] waveSpawners; // Array of paths to EnemySpawner nodes
	[Export] public float waveInterval = 10.0f; // Time between waves

	private int _currentWave = 0;
	private float _waveTimer = 0.0f;
	private float viewportHeight;

	public override void _Ready()
	{
		viewportHeight = GetViewportRect().Size.Y;
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint()) return;

		if (waveBackground.Position.Y <= viewportHeight)
		{
			Vector2 currentPos = waveBackground.Position;
			currentPos.Y = Mathf.Lerp(currentPos.Y, 0, (float)delta / (waveInterval * waveSpawners.Length));
			waveBackground.Position = currentPos;
		}

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
