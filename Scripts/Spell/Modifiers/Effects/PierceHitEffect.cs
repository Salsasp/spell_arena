public sealed class PierceHitEffect : IModifierEffect
{
	public string EffectId => "pierce";
	public ModifierPhase Phase => ModifierPhase.OnHit;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		context.HitResult.PreventDespawn = true;
	}
}
