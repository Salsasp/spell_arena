using Godot;

public partial class OrbitProjectile : Node2D
{
	[Export] public float OrbitDistance = 50f;
	[Export] public float AngularSpeed = 4f;
	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private HitComponent hitComponent;
	private SpellMovementComponent spellMovementComponent;
	private SpellSpawnComponent spawnComponent;
	private ModifierManager modifierManager;
	private AnimationComponent animationComponent;
	


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

        spellMovementComponent.Update(delta);
		GlobalPosition += spellMovementComponent.Velocity * (float)delta;
    }

}
