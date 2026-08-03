public interface IModifierEffect
{
	string EffectId { get; }
	ModifierPhase Phase { get; }
	void Apply(ModifierContext context, ModifierData modifier);
}
