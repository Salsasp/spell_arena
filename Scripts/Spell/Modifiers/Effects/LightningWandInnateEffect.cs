public sealed class LightningWandInnateEffect : IModifierEffect
{
	public string EffectId => "lightning_wand_innate";
	public ModifierPhase Phase => ModifierPhase.OnHit;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		if (context.HitTarget == null || context.Damage == null)
		{
			return;
		}

		LightningChain.Strike(context.HitTarget, context.Damage, modifier.copies);
	}
}
