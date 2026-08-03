using Godot;
using System;

public sealed class OrbitSpawnEffect : IModifierEffect
{
	public string EffectId => "orbit";
	public ModifierPhase Phase => ModifierPhase.OnSpawn;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		if (context.Spawn != null && !context.Spawn.OrbitAllowed)
		{
			return;
		}

		if (context.OrbitScene == null || context.Projectile == null)
		{
			return;
		}

		if (context.Projectile.GetNodeOrNull<OrbitManager>("OrbitManager") != null)
		{
			return;
		}

		try
		{
			OrbitManager orbitManager = context.OrbitScene.Instantiate<OrbitManager>();
			orbitManager.Name = "OrbitManager";
			orbitManager.ConfigureFromModifiers(context.ModifierManager);
			context.Projectile.AddChild(orbitManager);
		}
		catch (Exception exception)
		{
			GD.PrintErr(exception);
		}
	}
}
