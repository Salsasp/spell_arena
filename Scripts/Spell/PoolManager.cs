using Godot;
using System;

public partial class PoolManager : Node2D
{
	[Export] public PackedScene BurnPoolScene;
	public static PoolManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public void SpawnBurnPool(Vector2 pos, double duration = -1)
	{
		BurnPool burnPool = BurnPoolScene.Instantiate<BurnPool>();
		AddChild(burnPool);
		burnPool.GlobalPosition = pos;
		if (duration != -1) burnPool.FirePoolDamageCooldown = duration;
	}
}
