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

		context.Hit.ApplyDot(context.HitTarget, modifier);
	}
}
