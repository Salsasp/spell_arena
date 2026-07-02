using Godot;

public partial class ProjectileSpawner : Node2D
{
	[Export] public PackedScene ProjectilePrefab { get; set; }

	public override void _Ready()
	{
		
	}

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

}