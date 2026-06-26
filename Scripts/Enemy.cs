using Godot;

public partial class Enemy : CharacterBody2D
{
    private HealthComponent healthComponent;
    private HitboxComponent hitboxComponent;

    public override void _Ready()
    {
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");

        // Connect signals
        healthComponent.HealthChanged += OnHealthChanged;
        healthComponent.Dead += OnDead;
        hitboxComponent.Hit += OnHit;
    }

    private void OnHealthChanged(int newHealth)
    {
        // TODO: Update UI, play animation, etc.
        GD.Print($"Health changed: {newHealth}");
    }

    private void OnHit(Node2D hitBy)
    {
        Modulate = Colors.Red;
		DamageComponent hitByDamageComponent = hitBy.GetNodeOrNull<DamageComponent>("DamageComponent");
        HitComponent hitByHitComponent = hitBy.GetNodeOrNull<HitComponent>("HitComponent");
		if (hitByDamageComponent == null || hitByHitComponent == null)
		{
			GD.PrintErr("Hit by area without valid component");
			return;
		}
		healthComponent.TakeDamage(hitByDamageComponent.CalculateDamage());
        hitByHitComponent.Update();
    }

    private void OnDead()
    {
        GD.Print("Enemy died!");
        // TODO: Play death animation, drop loot, etc.
    }
}