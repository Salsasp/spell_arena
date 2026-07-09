using System;
using Godot;

public partial class Enemy : CharacterBody2D
{
    private HealthComponent healthComponent;
    private HitboxComponent hitboxComponent;
    private MovementComponent movementComponent;
    private AIMovementComponent aiMovementComponent;
    private ProjectileSpawner projectileSpawner;
    private Player player;

    public override void _Ready()
    {
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
        movementComponent = GetNode<MovementComponent>("MovementComponent");
        aiMovementComponent = GetNode<AIMovementComponent>("AIMovementComponent");
        projectileSpawner = GetNode<ProjectileSpawner>("ProjectileSpawner");
        player = GetNodeOrNull<Player>("../../Player");
        aiMovementComponent.Initialize(movementComponent, player);

        // Connect signals
        healthComponent.HealthChanged += OnHealthChanged;
        healthComponent.Dead += OnDead;
        hitboxComponent.Hit += OnHit;
    }

    public override void _PhysicsProcess(double delta)
    {
        movementComponent.Update(delta);
    }


    private void OnHealthChanged(int newHealth)
    {
        // TODO: Update UI, play animation, etc.
        GD.Print($"Health changed: {newHealth}");
    }

    private void OnHit(Area2D hitBy)
    {
        if (hitBy.Name != "ProjArea2D")
        {
            return;
        }
        Modulate = Colors.Red;
        Node2D root = hitBy.GetParent<Node2D>();
		DamageComponent hitByDamageComponent = root.GetNodeOrNull<DamageComponent>("DamageComponent");
        HitComponent hitByHitComponent = root.GetNodeOrNull<HitComponent>("HitComponent");
		if (hitByDamageComponent == null || hitByHitComponent == null)
		{
			GD.PrintErr("Hit by area without valid component");
			return;
		}
		healthComponent.TakeDamage(hitByDamageComponent.CalculateDamage());
        hitByHitComponent.Update(this);
    }

    private void OnDead()
    {
        GD.Print("Enemy died!");
        // TODO: Play death animation, drop loot, etc.
    }

    public bool DotApplied()
    {
        foreach (Node child in this.GetChildren())
        {
            if (child is DotComponent)
            {
                return true;
            }
        }

        return false;
    }
}