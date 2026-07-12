using Godot;

public partial class ProjectileSpawner : Node2D
{
	[Export] public PackedScene ProjectilePrefab { get; set; }
	[Export] public PackedScene EnemyProjectilePrefab { get; set; }

	public void SpawnProjectile(Vector2 pos, Vector2 dir, Vector2 initVel = new())
	{
		Node2D projectile = ProjectilePrefab.Instantiate<Node2D>();
		GetParent().AddChild(projectile);
		projectile.Position = pos;

		var spellMovementComponent = projectile.GetNode<SpellMovementComponent>("SpellMovementComponent");
		spellMovementComponent.Direction = dir;

		if (initVel != Vector2.Zero)
		{
			spellMovementComponent.Velocity = initVel;
		}

		var projectileArea = projectile.GetNode<Area2D>("ProjArea2D");
		projectileArea.Rotation = dir.Angle();
	}

	public void SpawnEnemyProjectile(Vector2 pos, Vector2 dir, int damage = 15)
	{
		Node2D projectile = EnemyProjectilePrefab.Instantiate<Node2D>();
		GetParent().AddChild(projectile);
		projectile.Position = pos;

		var spellMovementComponent = projectile.GetNode<SpellMovementComponent>("SpellMovementComponent");
		spellMovementComponent.Direction = dir;

		var projectileArea = projectile.GetNode<Area2D>("ProjArea2D");
		projectileArea.Rotation = dir.Angle();

		var damageComponent = projectile.GetNode<DamageComponent>("DamageComponent");
		damageComponent.BaseDamage = damage;
	}
}
