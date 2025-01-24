using Godot;
using System;
using System.Collections.Generic;



public partial class BulletManager : Singleton<BulletManager>
{
	[Export] public EnemyConfig[] enemyConfigs;

	Dictionary<EnemyType, EnemyConfig> enemyDict = new Dictionary<EnemyType, EnemyConfig>();

	public override void _Ready()
	{
		foreach (var config in enemyConfigs)
		{
			enemyDict.Add(config.enemyType, config);
		}
	}

	public BubbleSettings GetBubbleSettingsByEnemey(EnemyType enemyType) => enemyDict[enemyType].bubbleSettings;

	public EnemyConfig GetEnemySettingsByEnemey(EnemyType enemyType) => enemyDict[enemyType];
}
