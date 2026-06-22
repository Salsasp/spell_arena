using Godot;
using System;

public partial class DamageComponent : Component
{
	[Export] public float CritChance { get; set; } = 0.05f; // 5 percent
	[Export] public int CritMultiplier { get; set; } = 2;
	[Export] public int BaseDamage { get; set; } = 20;
	
	public override void _Ready()
	{
	}

	public int CalculateDamage()
	{
		int dmg = BaseDamage;
		if (IsCriticalHit())
		{
			dmg *= CritMultiplier;
		}
		
		return dmg;
	}

	public bool IsCriticalHit()
	{
		if (CritChance >= 1f)
			return true;
		
		return GD.Randf() < CritChance;
	}
}
