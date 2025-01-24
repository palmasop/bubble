using Godot;
using System;
using System.Collections.Generic;



public partial class BulletManager : Singleton<BulletManager>
{
	[Export] public EnemyConfig[] enemyConfigs;

	Dictionary<EnemyType, BubbleSettings> enemyDict = new Dictionary<EnemyType, BubbleSettings>();

	public override void _Ready()
	{
		foreach (var config in enemyConfigs)
		{
			enemyDict.Add(config.enemyType, config.bubbleSettings);
		}
	}

	public BubbleSettings GetBubbleSettingsByEnemey(EnemyType enemyType)
	{
		return enemyDict[enemyType];
	}
}
