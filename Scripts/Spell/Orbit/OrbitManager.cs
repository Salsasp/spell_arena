using Godot;
using System;
using System.Collections.Generic;

public partial class OrbitManager : Node2D
{
	[Export] public PackedScene OrbitProjectileScene;
	[Export] public PackedScene OrbitFireballScene;
	[Export] public PackedScene OrbitWizardScene;
	[Export] public int ProjectilesPerModifier = 3;
	[Export] public float OrbitDistance = 50f;
	public List<OrbitProjectile> OrbitProjectiles = [];
	public bool hasFireballs = false;
	public bool hasWizards = false;
	private Projectile parentProjectile;

	public override void _Ready()
	{
		try
		{
			parentProjectile = GetParent<Projectile>();
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}

		InitOrbitProjectiles();
	}

	private void InitOrbitProjectiles()
	{
		if (hasFireballs && OrbitFireballScene != null)
		{
			for (int i = 0; i < ProjectilesPerModifier; i++)
			{
				OrbitFireball fireball = OrbitFireballScene.Instantiate<OrbitFireball>();
				OrbitProjectiles.Add(fireball);
			}
		}

		if (hasWizards && OrbitWizardScene != null)
		{
			for (int i = 0; i < ProjectilesPerModifier; i++)
			{
				OrbitWizard wizard = OrbitWizardScene.Instantiate<OrbitWizard>();
				OrbitProjectiles.Add(wizard);
			}
		}

		if (OrbitProjectiles.Count == 0)
		{
			return;
		}

		float angleOffset = 2f * Mathf.Pi / OrbitProjectiles.Count;
		for (int i = 0; i < OrbitProjectiles.Count; i++)
		{
			OrbitProjectile orbitProjectile = OrbitProjectiles[i];
			AddChild(orbitProjectile);
			orbitProjectile.InitializeFromParent(parentProjectile);
			orbitProjectile.OrbitDistance = OrbitDistance;
			Vector2 pos = new(Mathf.Cos(angleOffset * i) * OrbitDistance, Mathf.Sin(angleOffset * i) * OrbitDistance);
			orbitProjectile.Position = pos;
			orbitProjectile.AngleToParent = angleOffset * i;
		}
	}
}
