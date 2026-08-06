public sealed class SwiftStepAcquireEffect : IModifierEffect
{
	public string EffectId => "swiftness";
	public ModifierPhase Phase => ModifierPhase.OnAcquire;
	public const float SWIFTNESS_INCREASE = 0.1f;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		context.ModifierManager.GetParent<Player>().GetNode<MovementComponent>("MovementComponent").SpeedMultiplier += SWIFTNESS_INCREASE;
	}
}
