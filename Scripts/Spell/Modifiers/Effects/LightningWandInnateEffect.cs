public sealed class LightningWandInnateEffect : IModifierEffect
{
	public string EffectId => "chain_lightning";
	public ModifierPhase Phase => ModifierPhase.OnHit;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		
	}
}
