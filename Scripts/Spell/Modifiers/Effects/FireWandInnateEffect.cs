using Godot;
using System;

public sealed class FireWandInnateEffect : IModifierEffect
{
	public string EffectId => "fire_wand_innate"; // TODO: fill this with valid modifier once created
	public ModifierPhase Phase => ModifierPhase.OnSpawn;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
        if (context.Hit == null || context.HitTarget == null)
		{
			return;
		}

        // Apply the burn dot
		context.Hit.ApplyDot(context.HitTarget, modifier);
        
        // TODO: create scene node burn pool
	}
}
