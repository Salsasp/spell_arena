public sealed class HomingSpawnEffect : IModifierEffect
{
	public string EffectId => "homing";
	public ModifierPhase Phase => ModifierPhase.OnSpawn;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		context.Movement.SetHomingEnabled(true, context.Movement.HomingTurnSpeed);
	}
}
