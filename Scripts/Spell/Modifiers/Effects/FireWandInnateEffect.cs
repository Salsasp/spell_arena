using Godot;

public sealed class FireWandInnateEffect : IModifierEffect
{
	public string EffectId => "fire_wand_innate";
	public ModifierPhase Phase => ModifierPhase.OnHit;

	public void Apply(ModifierContext context, ModifierData modifier)
	{
		if (context.Hit == null || context.HitTarget == null)
		{
			return;
		}

		context.Hit.ApplyDot(context.HitTarget, modifier);

		if (context.HitTarget is Enemy enemy)
		{
			// Recorded on the enemy because the pool only spawns once it dies. Highest
			// stack count wins so a later, weaker hit can never shrink a pending pool.
			enemy.BurnPoolStacks = Mathf.Max(enemy.BurnPoolStacks, modifier.copies);
		}
	}
}
