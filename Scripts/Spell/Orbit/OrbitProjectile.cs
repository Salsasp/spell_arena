using Godot;

public partial class OrbitProjectile : Node2D
{
	public float OrbitDistance;
	[Export] public float AngularSpeed = 4f;
	public float AngleToParent;

	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private HitComponent hitComponent;
	private SpellMovementComponent spellMovementComponent;
	private AnimationComponent animationComponent;

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

	public void InitializeFromParent(Projectile parentProjectile)
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
