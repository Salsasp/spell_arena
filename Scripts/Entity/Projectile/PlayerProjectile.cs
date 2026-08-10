using Godot;
using System;
using System.ComponentModel;

public partial class PlayerProjectile : Node2D, IProjectile
{
	[Export] public PackedScene OrbitScene { get; set; }
	[Export] public DamageComponent damageComponent { get; set; }
	[Export] public DespawnComponent despawnComponent { get; set; }
	[Export] public HitComponent hitComponent { get; set; }
	[Export] public LifetimeComponent lifetimeComponent { get; set; }
	[Export] public SpellMovementComponent spellMovementComponent { get; set; }
	[Export] public SpellSpawnComponent spawnComponent { get; set; }
	public ModifierManager modifierManager { get; set; }
	[Export] public AnimationComponent animationComponent { get; set; }
	[Export] public Area2D projArea2D { get; set; }


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
