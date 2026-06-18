using Godot;
using System;

public partial class DamageComponent : Node
{
	[Export] public float CritChance { get; set; } = 0.05f; // 5 percent
	[Export] public int CritMultiplier { get; set; } = 2;
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public bool IsCriticalHit()
	{
		if (CritChance >= 1f)
			return true;
		
		return GD.Randf() < CritChance;
	}
}
