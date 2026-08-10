using Godot;

public partial class OrbitProjectile : PlayerProjectile
{
	public float OrbitDistance;
	[Export] public float AngularSpeed = 4f;
	public float AngleToParent;

	public override void _Ready()
	{
		EnsureComponentsInitialized();
		animationComponent.Update();
	}

	public override void _PhysicsProcess(double delta)
	{
		AngleToParent += AngularSpeed * (float)delta;
		Position = Vector2.FromAngle(AngleToParent) * OrbitDistance;
	}

	public void InitializeFromParent(PlayerProjectile parentProjectile)
	{
		EnsureComponentsInitialized();

		if (parentProjectile?.modifierManager == null)
		{
			GD.PrintErr($"{Name}: missing parent modifier manager.");
			return;
		}

		hitComponent.Initialize(parentProjectile.modifierManager, despawnComponent, spellMovementComponent);
	}

	private void EnsureComponentsInitialized()
	{
		damageComponent ??= GetNode<DamageComponent>("DamageComponent");
		despawnComponent ??= GetNode<DespawnComponent>("DespawnComponent");
		hitComponent ??= GetNode<HitComponent>("HitComponent");
		spellMovementComponent ??= GetNode<SpellMovementComponent>("SpellMovementComponent");
		animationComponent ??= GetNode<AnimationComponent>("AnimationComponent");
	}
}
