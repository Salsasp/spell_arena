using Godot;

public partial class EnemyProjectile : Node2D
{
	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private LifetimeComponent lifetimeComponent;
	private SpellMovementComponent spellMovementComponent;

	public override void _Ready()
	{
		damageComponent = GetNode<DamageComponent>("DamageComponent");
		despawnComponent = GetNode<DespawnComponent>("DespawnComponent");
		lifetimeComponent = GetNode<LifetimeComponent>("LifetimeComponent");
		spellMovementComponent = GetNode<SpellMovementComponent>("SpellMovementComponent");
	}

	public override void _PhysicsProcess(double delta)
	{
		spellMovementComponent.Update();
		GlobalPosition += spellMovementComponent.Velocity * (float)delta;

		if (lifetimeComponent.TimedOut)
		{
			despawnComponent.Despawn();
		}
	}
}
