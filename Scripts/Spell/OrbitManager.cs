using Godot;
using System;
using System.Collections.Generic;

public partial class OrbitManager : Node2D
{
	[Export] public PackedScene OrbitProjectileScene;
	[Export] public int ProjectilesPerModifier = 3;
	[Export] public float OrbitDistance = 50f;
	public List<Node2D> OrbitProjectiles = [];
	public bool hasFireballs = false;
	public bool hasWizards = false;
	private Node2D parentProjectile;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try
		{
			parentProjectile = GetParent<Node2D>();
		}
		catch(Exception e)
		{
			GD.PrintErr(e);
		}

		InitOrbitProjectiles();
	}

	private void InitOrbitProjectiles()
	{
		if (hasFireballs)
		{
			float angleOffset = (2f*Mathf.Pi)/ProjectilesPerModifier;
			for (int i = 0; i < ProjectilesPerModifier; i++)
			{
				OrbitProjectile orbitProjectile = OrbitProjectileScene.Instantiate<OrbitProjectile>();
				Vector2 pos = new(Mathf.Cos(angleOffset*i)*OrbitDistance, Mathf.Sin(angleOffset*i)*OrbitDistance);
				orbitProjectile.GlobalPosition = pos;
				orbitProjectile.AngleToParent = angleOffset;
				// TODO: set data for fireballs, wizards, etc
			}
		}
	}
}
