using Godot;

public partial class MeleeAttackComponent : AttackBehaviorComponent
{
	[Export] public float AttackDelay = 1.0f;
	[Export] public float MeleeRange = 40f;
	[Export] public int Damage = 10;

	private float cooldown;

	public override void Initialize(EnemyContext ctx)
	{
		base.Initialize(ctx);
		cooldown = AttackDelay;
	}

	public override void Update(double delta)
	{
		cooldown -= (float)delta;

		if (cooldown > 0 || context == null || !context.IsPlayerValid)
		{
			return;
		}

		float distance = context.GlobalPosition.DistanceTo(context.Player.GlobalPosition);
		if (distance > MeleeRange)
		{
			return;
		}

		HealthComponent playerHealth = context.Player.GetNode<HealthComponent>("HealthComponent");
		playerHealth.TakeDamage(Damage);
		cooldown = AttackDelay;
	}
}
