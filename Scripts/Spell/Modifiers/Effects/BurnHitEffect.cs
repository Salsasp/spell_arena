public sealed class BurnHitEffect : IModifierEffect
{
	public string EffectId => "apply_burn";
	public ModifierPhase Phase => ModifierPhase.OnHit;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		if (context.Hit == null || context.HitTarget == null)
		{
			return;
		}

		// Burn pools are exclusive to the fire wand innate; this only applies the damage over time
		context.Hit.ApplyDot(context.HitTarget, modifier);
	}
}
