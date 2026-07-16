using Godot;

public partial class ProjectileSpawner : Node2D
{
	[Export] public PackedScene ProjectilePrefab { get; set; }
	[Export] public PackedScene EnemyProjectilePrefab { get; set; }
	[Export] public PackedScene TextProjectilePrefab { get; set; }

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

	public void SpawnTextProjectile(Vector2 pos, Vector2 dir, string text = "", int damage = 1)
	{
		TextProjectile projectile = TextProjectilePrefab.Instantiate<TextProjectile>();
		projectile.Text = text;
		projectile.Position = pos;

		var spellMovementComponent = projectile.GetNode<SpellMovementComponent>("SpellMovementComponent");
		spellMovementComponent.Direction = dir;

		var projectileArea = projectile.GetNode<Area2D>("ProjArea2D");
		projectileArea.Rotation = dir.Angle();

		var damageComponent = projectile.GetNode<DamageComponent>("DamageComponent");
		damageComponent.BaseDamage = damage;

		GetParent().CallDeferred(Node.MethodName.AddChild, projectile);
	}
}
