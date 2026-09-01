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

	public void SpawnBurnPool(Vector2 pos, double duration = -1, int stacks = 1)
	{
		BurnPool burnPool = BurnPoolScene.Instantiate<BurnPool>();

		// Configured before AddChild, since AddChild runs _Ready and that is where the
		// stack scaling and the damage timer are read.
		burnPool.Stacks = stacks;
		if (duration != -1) burnPool.FirePoolDamageCooldown = duration;

		AddChild(burnPool);
		burnPool.GlobalPosition = pos;
	}
}
