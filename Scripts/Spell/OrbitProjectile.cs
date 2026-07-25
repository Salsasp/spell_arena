using Godot;

public partial class OrbitProjectile : Node2D
{
	public float OrbitDistance;
	[Export] public float AngularSpeed = 4f;
	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private HitComponent hitComponent;
	private SpellMovementComponent spellMovementComponent;
	private AnimationComponent animationComponent;
	public float AngleToParent;

	public override void _Ready()
	{
		damageComponent = GetNode<DamageComponent>("DamageComponent");
		despawnComponent = GetNode<DespawnComponent>("DespawnComponent");
		hitComponent = GetNode<HitComponent>("HitComponent");
		spellMovementComponent = GetNode<SpellMovementComponent>("SpellMovementComponent");
		animationComponent = GetNode<AnimationComponent>("AnimationComponent");

		animationComponent.Update();
	}

	public override void _PhysicsProcess(double delta)
	{
		AngleToParent += AngularSpeed * (float)delta;
		Position = Vector2.FromAngle(AngleToParent) * OrbitDistance;
	}

	public void InitializeFromParent(Projectile parentProjectile)
	{
		ModifierManager modifierManager = parentProjectile.modifierManager;
		hitComponent.Initialize(modifierManager, despawnComponent, spellMovementComponent);
	}
}
