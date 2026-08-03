public sealed class ExtraDraftOptionAcquireEffect : IModifierEffect
{
	public string EffectId => "extra_draft_option";
	public ModifierPhase Phase => ModifierPhase.OnAcquire;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		context.ModifierManager.ExtraDraftOptions += 1;
	}
}
