using Godot;
using System;

public partial class Projectile : Node2D
{
	private DamageComponent _damageComponent;
	private DespawnComponent _despawnComponent;
	private HitComponent _hitComponent;
	private LifetimeComponent _lifetimeComponent;
	private SpellMovementComponent _spellMovementComponent;

	public override void _Ready()
	{
		_damageComponent = GetNode<DamageComponent>("../DamageComponent");
		_despawnComponent = GetNode<DespawnComponent>("../DespawnComponent");
		_hitComponent = GetNode<HitComponent>("../HitComponent");
		_lifetimeComponent = GetNode<LifetimeComponent>("../LifetimeComponent");
		_spellMovementComponent = GetNode<SpellMovementComponent>("../SpellMovementComponent");
	}

    public override void _PhysicsProcess(double delta)
    {
        _spellMovementComponent.Update();
		GlobalPosition += _spellMovementComponent.Velocity * (float)delta;
		//GD.Print("Pos:" + Position);
		//GD.Print("Rotation" + Rotation);
    }

}
