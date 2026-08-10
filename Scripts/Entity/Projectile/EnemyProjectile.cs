using Godot;

public partial class EnemyProjectile : Node2D, IProjectile
{
	[Export] public DamageComponent damageComponent { get; set; }
	[Export] public DespawnComponent despawnComponent { get; set; }
	[Export] public LifetimeComponent lifetimeComponent { get; set; }
	[Export] public SpellMovementComponent spellMovementComponent { get; set; }
	[Export] public AnimationComponent animationComponent { get; set; }

	public override void _Ready()
	{
		damageComponent = GetNode<DamageComponent>("DamageComponent");
		despawnComponent = GetNode<DespawnComponent>("DespawnComponent");
		lifetimeComponent = GetNode<LifetimeComponent>("LifetimeComponent");
		spellMovementComponent = GetNode<SpellMovementComponent>("SpellMovementComponent");
	}

	public override void _PhysicsProcess(double delta)
	{
		spellMovementComponent.Update(delta);
		GlobalPosition += spellMovementComponent.Velocity * (float)delta;

		if (lifetimeComponent.TimedOut)
		{
			despawnComponent.Despawn();
		}
	}
}
