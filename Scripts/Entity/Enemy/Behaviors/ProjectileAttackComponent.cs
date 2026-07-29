using Godot;

public partial class ProjectileAttackComponent : AttackBehaviorComponent
{
	[Export] public float AttackDelay = 2.0f;
	[Export] public int Damage = 15;

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

		Vector2 projDir = context.GlobalPosition.DirectionTo(context.Player.GlobalPosition);
		context.ProjectileSpawner.SpawnEnemyProjectile(context.GlobalPosition, projDir, Damage);
		cooldown = AttackDelay;
	}
}
