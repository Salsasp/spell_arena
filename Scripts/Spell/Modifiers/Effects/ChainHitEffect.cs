public sealed class ChainHitEffect : IModifierEffect
{
	public string EffectId => "chain";
	public ModifierPhase Phase => ModifierPhase.OnHit;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		if (context.Hit == null || context.HitTarget == null || !context.Hit.ChainEnabled)
		{
			return;
		}

		context.Hit.ChainSpell(context.HitTarget);
		context.HitResult.HandledChain = true;
	}
}
