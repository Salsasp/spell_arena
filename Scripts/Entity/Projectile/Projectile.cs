using Godot;
using System;
using System.ComponentModel;

public partial class Projectile : Node2D
{
	[Export] public PackedScene OrbitScene;
	[Export] public DamageComponent damageComponent;
	[Export] public DespawnComponent despawnComponent;
	[Export] public HitComponent hitComponent;
	[Export] public LifetimeComponent lifetimeComponent;
	[Export] public SpellMovementComponent spellMovementComponent;
	[Export] public SpellSpawnComponent spawnComponent;
	public ModifierManager modifierManager;
	[Export] public AnimationComponent animationComponent;
	[Export] public Area2D projArea2D;


	public override void _Ready()
	{
		modifierManager = GetNode<ModifierManager>("../Player/ModifierManager");
		animationComponent.Update();

		hitComponent.Initialize(modifierManager, despawnComponent, spellMovementComponent);
		spawnComponent.Initialize(modifierManager, damageComponent);

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
