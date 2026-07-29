using Godot;
using System;
using System.ComponentModel;

public partial class Projectile : Node2D
{
	[Export] public PackedScene OrbitScene;
	public DamageComponent damageComponent;
	public DespawnComponent despawnComponent;
	public HitComponent hitComponent;
	public LifetimeComponent lifetimeComponent;
	public SpellMovementComponent spellMovementComponent;
	public SpellSpawnComponent spawnComponent;
	public ModifierManager modifierManager;
	public AnimationComponent animationComponent;


	public override void _Ready()
	{
		damageComponent = GetNode<DamageComponent>("DamageComponent");
		despawnComponent = GetNode<DespawnComponent>("DespawnComponent");
		hitComponent = GetNode<HitComponent>("HitComponent");
		lifetimeComponent = GetNode<LifetimeComponent>("LifetimeComponent");
		spellMovementComponent = GetNode<SpellMovementComponent>("SpellMovementComponent");
		spawnComponent = GetNode<SpellSpawnComponent>("SpellSpawnComponent");
		modifierManager = GetNode<ModifierManager>("../Player/ModifierManager");
		animationComponent = GetNode<AnimationComponent>("AnimationComponent");

		animationComponent.Update();

		hitComponent.Initialize(modifierManager, despawnComponent, spellMovementComponent);
		spawnComponent.Initialize(modifierManager, damageComponent, spellMovementComponent, OrbitScene);

		spawnComponent.Update();
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
