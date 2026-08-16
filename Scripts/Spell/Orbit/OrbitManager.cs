using Godot;
using System;
using System.Collections.Generic;

public partial class OrbitManager : Node2D
{
	private readonly struct OrbitTypeDefinition
	{
		public readonly string OrbitalKey;
		public readonly PackedScene Scene;

		public OrbitTypeDefinition(string orbitalKey, PackedScene scene)
		{
			OrbitalKey = orbitalKey;
			Scene = scene;
		}
	}

	[Export] public PackedScene OrbitProjectileScene;
	[Export] public PackedScene OrbitFireballScene;
	[Export] public PackedScene OrbitWizardScene;
	[Export] public int ProjectilesPerModifier = 3;
	[Export] public float OrbitDistance = 50f;

	public List<OrbitProjectile> OrbitProjectiles = [];

	private readonly HashSet<string> activeOrbitalKeys = [];
	private PlayerProjectile parentProjectile;

	public void ConfigureFromModifiers(IEnumerable<ModifierData> modifiers)
	{
		activeOrbitalKeys.Clear();

		if (modifiers == null)
		{
			return;
		}

		foreach (ModifierData modifier in modifiers)
		{
			if (modifier.onSpawn == "orbit" && !string.IsNullOrEmpty(modifier.orbital))
			{
				activeOrbitalKeys.Add(modifier.orbital);
			}
		}
	}

	public override void _Ready()
	{
		try
		{
			parentProjectile = GetParent<PlayerProjectile>();
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}

		InitOrbitProjectiles();
	}

	private IEnumerable<OrbitTypeDefinition> GetOrbitTypeDefinitions()
	{
		yield return new OrbitTypeDefinition("fireball", OrbitFireballScene);
		yield return new OrbitTypeDefinition("wizard", OrbitWizardScene);
	}

	private void InitOrbitProjectiles()
	{
		PopulateOrbit();

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

	private void PopulateOrbit()
	{
		OrbitProjectiles.Clear();

		List<PackedScene> activeScenes = [];
		foreach (OrbitTypeDefinition orbitType in GetOrbitTypeDefinitions())
		{
			if (!activeOrbitalKeys.Contains(orbitType.OrbitalKey) || orbitType.Scene == null)
			{
				continue;
			}

			activeScenes.Add(orbitType.Scene);
		}

		if (activeScenes.Count == 0)
		{
			return;
		}

		int totalCount = ProjectilesPerModifier * activeScenes.Count;
		OrbitProjectiles.Capacity = totalCount;

		for (int i = 0; i < totalCount; i++)
		{
			PackedScene scene = activeScenes[i % activeScenes.Count];
			OrbitProjectile projectile = scene.Instantiate<OrbitProjectile>();
			OrbitProjectiles.Add(projectile);
		}
	}
}
