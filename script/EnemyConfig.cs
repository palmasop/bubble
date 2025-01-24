using System.Collections.Generic;
using Godot;

[Tool]
[GlobalClass]
public partial class EnemyConfig : Resource
{
    [Export] public EnemyType enemyType;
    [Export] public BubbleSettings bubbleSettings;
}
