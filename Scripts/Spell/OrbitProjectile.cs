using Godot;

public partial class OrbitProjectile : Node2D
{
	private float orbitDistance = 50f;
	[Export] public float AngularSpeed = 4f;
	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private HitComponent hitComponent;
	private SpellMovementComponent spellMovementComponent;
	private SpellSpawnComponent spawnComponent;
	private ModifierManager modifierManager;
	private AnimationComponent animationComponent;
	public float AngleToParent = 0f;
	


	public override void _Ready()
	{
		damageComponent = GetNode<DamageComponent>("DamageComponent");
		despawnComponent = GetNode<DespawnComponent>("DespawnComponent");
		hitComponent = GetNode<HitComponent>("HitComponent");
		spellMovementComponent = GetNode<SpellMovementComponent>("SpellMovementComponent");
		spawnComponent = GetNode<SpellSpawnComponent>("SpellSpawnComponent");
		modifierManager = GetNode<ModifierManager>("../Player/ModifierManager");
		animationComponent = GetNode<AnimationComponent>("AnimationComponent");

		animationComponent.Update();

		hitComponent.Initialize(modifierManager, despawnComponent, spellMovementComponent);
		spawnComponent.Initialize(modifierManager, damageComponent, spellMovementComponent);

		spawnComponent.Update();
	}

    public override void _PhysicsProcess(double delta)
    {
		// Rotation around parent
		AngleToParent += AngularSpeed*(float)delta;
		spellMovementComponent.Velocity = new Vector2(Mathf.Cos(AngleToParent)*orbitDistance, Mathf.Sin(AngleToParent)*orbitDistance);

        spellMovementComponent.Update(delta);
		GlobalPosition += spellMovementComponent.Velocity * (float)delta;
    }

}
