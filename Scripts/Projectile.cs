using Godot;
using System;
using System.ComponentModel;

public partial class Projectile : Node2D
{
	private DamageComponent damageComponent;
	private DespawnComponent despawnComponent;
	private HitComponent hitComponent;
	private LifetimeComponent lifetimeComponent;
	private SpellMovementComponent spellMovementComponent;
	private SpellSpawnComponent spawnComponent;
	private ModifierManager modifierManager;
	private AnimationComponent animationComponent;


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
		spawnComponent.Initialize(modifierManager, damageComponent);

		spawnComponent.Update();
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
